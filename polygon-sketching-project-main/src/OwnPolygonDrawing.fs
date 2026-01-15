module OwnPolygonDrawing 

open Fable.Core
open Feliz
open Elmish
open Browser.Dom
open System

type Coord = { x : float; y : float }
type CanvasSize = { width : float; height : float }
type PolyLine = list<Coord>
let viewBoxWidth = 1000.0
let viewBoxHeight = 600.0

type Model = { 
    canvasSize : CanvasSize
    currentPolyline: Option<PolyLine>
    finishedPolygons: list<PolyLine>
    mousePos: Option<Coord>
    past: Option<Model>
    future: Option<Model>
}

type Msg =
    | SetCanvasSize of CanvasSize
    | MouseMove of float * float
    | AddPoint of float * float
    | FinishPolygon
    | Undo
    | Redo

let toSvgCoord (canvasSize: CanvasSize) (relX, relY) : Coord =
    let scaleX = viewBoxWidth / canvasSize.width
    let scaleY = viewBoxHeight / canvasSize.height
    { x = relX * scaleX; y = relY * scaleY }

let pointsToString (points : list<Coord>) =
    points
    |> List.rev
    |> List.map (fun p -> sprintf "%f,%f" p.x p.y)
    |> String.concat " "

// Elmish funcs 
// Elmish needs init : unit -> Model * Cmd<Msg>
let init () : Model * Cmd<Msg> =
    let m = 
        { canvasSize = { width = 1000.0; height = 600.0 }
          currentPolyline = None
          finishedPolygons = []
          mousePos = None
          past = None
          future = None }
    m, Cmd.none

// Elmish needs update : Msg -> Model -> Model * Cmd<Msg> (or Model * Cmd<Msg>
let update (msg : Msg) (model : Model) : Model * Cmd<Msg> =
    let snapshot (m : Model) = { m with mousePos = None }
    match msg with
    | MouseMove (x, y) ->
        let pos = toSvgCoord model.canvasSize (x, y)
        { model with mousePos = Some pos }, Cmd.none
    | AddPoint (x, y) ->
        let pos = toSvgCoord model.canvasSize (x, y)
        let nextPolyline =
            match model.currentPolyline with
            | None -> Some [pos]
            | Some vertices -> Some (pos :: vertices)
        let nextModel = { model with currentPolyline = nextPolyline }
        { nextModel with past = Some (snapshot model); future = None }, Cmd.none
    | FinishPolygon ->
        match model.currentPolyline with
        | None -> model, Cmd.none
        | Some vertices ->
            let nextModel =
                { model with
                    currentPolyline = None
                    finishedPolygons = vertices :: model.finishedPolygons }
            { nextModel with past = Some (snapshot model); future = None }, Cmd.none
    | Undo ->
        match model.past with
        | Some previous -> { previous with future = Some (snapshot model); mousePos = None }, Cmd.none
        | None -> model, Cmd.none
    | Redo ->
        match model.future with
        | Some next -> { next with past = Some (snapshot model); mousePos = None }, Cmd.none
        | None -> model, Cmd.none
    | _ -> model, Cmd.none

// Own Logic

let render (model: Model) (dispatch: Msg -> unit) = 
    let getRelativePos (mouseEvent: Browser.Types.MouseEvent) =
        let rect = (mouseEvent.currentTarget :?> Browser.Types.Element).getBoundingClientRect()
        let relX = mouseEvent.clientX - rect.left
        let relY = mouseEvent.clientY - rect.top
        relX, relY

    let previewPoints =
        match model.mousePos, model.currentPolyline with
        | Some pos, Some vertices when vertices <> [] -> pos :: vertices
        | _, Some vertices -> vertices
        | _ -> []

    let finishedPolylines =
        model.finishedPolygons
        |> List.filter (fun poly -> List.length poly > 1)
        |> List.map (fun poly ->
            Svg.polyline [
                svg.points (pointsToString poly)
                svg.fill "none"
                svg.stroke "green"
                svg.strokeWidth 2
            ]
        )

    let currentPolylineElement =
        match model.currentPolyline with
        | Some vertices when List.length vertices > 1 ->
            [ Svg.polyline [
                svg.points (pointsToString vertices)
                svg.fill "none"
                svg.stroke "red"
                svg.strokeWidth 2
              ] ]
        | _ -> []

    Html.div [
        prop.style [ 
            style.padding 20 
            style.fontFamily "SF Compact Rounded, serif"
            style.color "green"; style.textAlign.center
            style.backgroundColor "lightgray"
            style.minHeight (length.vh 100) 
            ]

        prop.children [
            Html.h1 "Custom Polygon Sketching App"
            Html.button [
                prop.style [style.margin 20]
                prop.className "liquid-btn"
                prop.onClick (fun _ -> dispatch Undo)
                prop.text "Undo"
            ]
            Html.button [
                prop.style [style.margin 20]
                prop.className "liquid-btn"
                prop.onClick (fun _ -> dispatch Redo)
                prop.text "Redo"
            ]
            Html.div [
                prop.style [ style.padding 16 ]
                prop.children [
                    Svg.svg [
                        svg.width model.canvasSize.width
                        svg.height model.canvasSize.height
                        svg.viewBox (0, 0, int viewBoxWidth, int viewBoxHeight)
                        svg.onMouseMove (fun mouseEvent ->
                            let relX, relY = getRelativePos mouseEvent
                            dispatch (MouseMove (relX, relY))
                        )
                        svg.onClick (fun mouseEvent ->
                            let relX, relY = getRelativePos mouseEvent
                            if mouseEvent.detail = 1 then
                                dispatch (AddPoint (relX, relY))
                            elif mouseEvent.detail = 2 then
                                dispatch FinishPolygon
                        )
                        svg.children (
                            List.concat [
                                [
                            Svg.rect [
                                svg.x 0
                                svg.y 0
                                svg.width viewBoxWidth
                                svg.height viewBoxHeight
                                svg.fill "white"
                                svg.stroke "black"
                                svg.strokeWidth 2
                                svg.rx 12
                                svg.ry 12
                            ]
                            if List.length previewPoints > 1 then
                                Svg.polyline [
                                svg.points (pointsToString previewPoints)
                                svg.fill "none"
                                svg.stroke "red"
                                svg.strokeWidth 2
                            ]
                            match model.mousePos with
                            | Some pos ->
                                Svg.circle [
                                    svg.cx pos.x
                                    svg.cy pos.y
                                    svg.r 3
                                    svg.fill "green"
                                ]
                            | None -> Html.none
                                ]
                                currentPolylineElement
                                finishedPolylines
                            ]
                        )
                    ]
                ]
            ]
        ]
    ]    
