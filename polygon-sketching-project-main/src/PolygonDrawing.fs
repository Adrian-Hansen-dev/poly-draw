// Example setup for the polygon drawing project.
// The application logics should be implemented in the `updateModel` function.
// The undo-redo-relevant parts should be implemented in `addUndoRedo`.abs
// Please note that, the logics is easiest implemented when always adding elements
// to the beginning of the list e.g. build polylines and polygons in reverse order.
module PolygonDrawing 

open Fable.Core
open Feliz
open Elmish
open Browser.Dom
open System

// we use a record here, a tuple could also serve the purpose though
type Coord = { x : float; y : float }
type CanvasSize = { width : float; height : float }

// "polygon" line. Each list element describes the respective vertex.
// note that we could use a record here, but a type-alias is more lightweight
// and serves its purpose.
// I recommend storing the coordinates in reverse order, so that each vertex gets prepended
// to the list. This way, adding new vertices is O(1).
type PolyLine = list<Coord>

type Model = {
    // all "finished" polygons, created so far, by convention, new PolyLines can be prepended to this list to
    // make additions efficent and the code more elegant.
    finishedPolygons : list<PolyLine>
    // the polygon, we are currently working on (and extending, vertex-by-vertex). Having the current
    // one explicitly as oposed to already in the finishedPolygons list makes the code a bit more elegant
    // and approachable
    currentPolygon : Option<PolyLine>
    // current positon of the mouse (to draw a preview)
    mousePos : Option<Coord>
    // optionally, the model before this current state (note, that this immutable!), used for redo
    past : Option<Model>
    // used for redo
    future : Option<Model>
    // current canvas size in SVG user units (px)
    canvasSize : CanvasSize
}

// and explicit representation of all possible user interactions. This one can be used for 
// automatic testing or storing interaction logs to disk
type Msg =
    | AddPoint of Coord
    | SetCursorPos of Option<Coord>
    | SetCanvasSize of CanvasSize
    | FinishPolygon
    | Undo
    | Redo

// creates the initial model, which is used when creating the interactive application (see Main.fs)
let canvasPadding = 22.0
let topReservedSpace = 180.0

let clampMin minValue value =
    if value < minValue then minValue else value

let getCanvasSize () =
    let width =
        float document.documentElement.clientWidth - (canvasPadding * 2.0)
        |> clampMin 0.0
    let height =
        float document.documentElement.clientHeight - topReservedSpace - (canvasPadding * 2.0)
        |> clampMin 0.0
    { width = width; height = height }

let init () =
    let size = getCanvasSize ()
    let m = 
        { finishedPolygons = []; currentPolygon = None; // records can be written multiline
          mousePos = None ; past = None; future = None; canvasSize = size }
    m, Cmd.none // Cmd is optionally to explicitly represent side-effects in a safe manner (here we don't bother)


(*
TODO: implement the core logics of the drawing app, which means:
Depending on the message,
For AddPoint mesages, add the point to the current polygon.
 - if there is no current polygon yet, create a new one with this point as its only vertex.
 - if there is already a polygon, prepend (or append if you like) it to the list of vertices
For FinishPolygon mesages:
 - if there is no current polygon (this means right click was used before even adding a single vertex), ignore the message
 - if there is a current polygon, reset the current polygon to None and add the current polygon as a new elemnet to finishedPolygons.
*)
let updateModel (msg : Msg) (model : Model) =
    match msg with
    | AddPoint coord ->
        match model.currentPolygon with
        | None ->
            { model with currentPolygon = Some [coord] }
        | Some vertices ->
            { model with currentPolygon = Some (coord :: vertices) }
    | FinishPolygon ->
        match model.currentPolygon with
        | None -> model
        | Some vertices ->
            { model with
                currentPolygon = None
                finishedPolygons = vertices :: model.finishedPolygons }
    | _ -> model

// wraps an update function with undo/redo.
let addUndoRedo (updateFunction : Msg -> Model -> Model) (msg : Msg) (model : Model) =
    // first let us, handle the cursor position, which is not undoable, and handle undo/redo messages
    // in a next step we actually run the "core" system logics.
    match msg with
    | SetCursorPos p -> 
        // update the mouse position and create a new model.
        { model with mousePos = p }
    | SetCanvasSize size ->
        { model with canvasSize = size }
    | Undo -> 
        match model.past with
        | Some previous -> { previous with future = Some model }
        | None -> model
    | Redo -> 
        match model.future with
        | Some next -> { next with past = Some model }
        | None -> model
    | _ -> 
        // use the provided update function for all remaining messages
        { updateFunction msg model with past = Some model; future = None }


let update (msg : Msg) (model : Model)  =
    let newModel = addUndoRedo updateModel msg model
    newModel, Cmd.none

[<Emit("getSvgCoordinates($0)")>] // wrapper to use the getSvgCoordinates JS function (provided by index.html) from f# here typesafely.
let getSvgCoordinates (o: Browser.Types.MouseEvent): Coord = jsNative

let viewPolygon (color : string) (points : PolyLine) =
    points 
    |> List.pairwise 
    |> List.map (fun (c0,c1) ->
        Svg.line [
            svg.x1 c0.x; svg.y1 c0.y
            svg.x2 c1.x; svg.y2 c1.y
            svg.stroke(color)
            svg.strokeWidth 2.0
            svg.strokeLineJoin "round"
        ]
    )
 

let render (model : Model) (dispatch : Msg -> unit) =
    Browser.Dom.console.log("PolygonDrawing.render", model)
    let size = model.canvasSize
    let borderInset = 1.0
    let borderWidth = clampMin 0.0 (size.width - (borderInset * 2.0))
    let borderHeight = clampMin 0.0 (size.height - (borderInset * 2.0))
    let borderRadius = 12.0
    let border = 
        Svg.rect [ // i used ; to group together attributes semantically.
            svg.x1 borderInset; svg.x2 (borderInset + borderWidth)
            svg.y1 borderInset; svg.y2 (borderInset + borderHeight)
            svg.width borderWidth; svg.height borderHeight
            svg.rx borderRadius; svg.ry borderRadius
            svg.stroke("black"); svg.strokeWidth(2); svg.fill "none"
        ] 

    // collect all svg elements of all finished polygons
    let finisehdPolygons = 
        model.finishedPolygons |> List.collect (viewPolygon "green")
    let currentPolygon =
        match model.currentPolygon with
        | None -> [] // if we have no polygon, create empty svg list
        | Some p -> 
            match model.mousePos with
            | None -> 
                viewPolygon "red" p
            | Some preview -> 
                // if we have a current mouse position, prepend the mouse position to the resulting polygon
                viewPolygon "red" (preview :: p)
 
    let svgElements = List.concat [finisehdPolygons; currentPolygon]

    Html.div [
        prop.style [
            style.custom("userSelect", "none")
            style.custom("display", "flex")
            style.custom("flexDirection", "column")
            style.custom("boxSizing", "border-box")
            style.custom("height", "100vh")
            style.custom("padding", "16px")
            style.custom("gap", "16px")
        ]
        prop.children [
            Html.div [
                prop.children [
                    Html.h1 "Polygon Sketching App"
                    Html.button [
                        prop.style [style.margin 20]; 
                        prop.onClick (fun _ -> dispatch Undo)
                        prop.children [Html.text "undo"]
                    ]
                    Html.button [
                        prop.style [style.margin 20]
                        prop.onClick (fun _ -> dispatch Redo)
                        prop.children [Html.text "redo"]
                    ]
                ]
            ]
            Svg.svg [
                svg.width size.width; svg.height size.height
                svg.onMouseMove (fun mouseEvent -> 
                    // compute SVG relative coordinates, using javascript function
                    let pos = getSvgCoordinates mouseEvent

                    // fable requires to "send" messages via side-effect. 
                    // Can be moved into UI system, e.g. see  https://elm-lang.org/examples/buttons
                    dispatch (SetCursorPos (Some pos))
                )
                svg.onClick (fun mouseEvent -> 
                    // create messages (purely descriptive)
                    let msgs = 
                        if mouseEvent.detail = 1 then
                            let pos = getSvgCoordinates mouseEvent
                            [AddPoint pos] 
                        elif mouseEvent.detail = 2 then
                            [FinishPolygon]
                        else
                            []

                    // fable requires to "send" messages via side-effect. 
                    // Can be moved into UI system, e.g. see  https://elm-lang.org/examples/buttons
                    msgs |> List.iter dispatch
                )
                svg.children (border :: svgElements) // use : to prepend the border to the other elements
            ]
        ]
    ]

let subscriptions (_model : Model) =
    let subscribe dispatch =
        let handler (_: Browser.Types.Event) =
            dispatch (SetCanvasSize (getCanvasSize ()))
        window.addEventListener("resize", handler)
        { new IDisposable with
            member _.Dispose() =
                window.removeEventListener("resize", handler)
        }
    [ ["window"; "resize"], subscribe ]
