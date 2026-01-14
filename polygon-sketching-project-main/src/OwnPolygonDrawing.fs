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
    currentPolyline: list<Coord>
    mousePos: Option<Coord>
}

type Msg =
    | SetCanvasSize of CanvasSize
    | MouseMove of float * float
    | AddPoint of float * float
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
          currentPolyline = []
          mousePos = None }
    m, Cmd.none

// Elmish needs update : Msg -> Model -> Model * Cmd<Msg> (or Model * Cmd<Msg>
let update (msg : Msg) (model : Model) : Model * Cmd<Msg> =
    match msg with
    | MouseMove (x, y) ->
        let pos = toSvgCoord model.canvasSize (x, y)
        { model with mousePos = Some pos }, Cmd.none
    | AddPoint (x, y) ->
        let pos = toSvgCoord model.canvasSize (x, y)
        { model with currentPolyline = pos :: model.currentPolyline }, Cmd.none
    | Undo -> model, Cmd.none
    | Redo -> model, Cmd.none

// Own Logic

let render (model: Model) (dispatch: Msg -> unit) = 
    let getRelativePos (mouseEvent: Browser.Types.MouseEvent) =
        let rect = (mouseEvent.currentTarget :?> Browser.Types.Element).getBoundingClientRect()
        let relX = mouseEvent.clientX - rect.left
        let relY = mouseEvent.clientY - rect.top
        relX, relY

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
                            dispatch (AddPoint (relX, relY))
                        )
                        svg.children [
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
                            if List.length model.currentPolyline > 1 then
                                Svg.polyline [
                                    svg.points (pointsToString model.currentPolyline)
                                    svg.fill "none"
                                    svg.stroke "black"
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
                    ]
                ]
            ]
        ]
    ]    
