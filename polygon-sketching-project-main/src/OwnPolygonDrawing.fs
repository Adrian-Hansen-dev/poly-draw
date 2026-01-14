module OwnPolygonDrawing 

open Fable.Core
open Feliz
open Elmish
open Browser.Dom
open System

type CanvasSize = { width : float; height : float }

// Model
type Model = { 
    canvasSize : CanvasSize
}

// Messages 
type Msg =
    | NoOp
    | Undo
    | Redo

// Elmish funcs 
// Elmish needs init : unit -> Model * Cmd<Msg>
let init () : Model * Cmd<Msg> =
    let m = 
        { canvasSize = { width = 1000.0; height = 600.0 } }
    m, Cmd.none

// Elmish needs update : Msg -> Model -> Model * Cmd<Msg> (or Model * Cmd<Msg>
let update (msg : Msg) (model : Model) : Model * Cmd<Msg> =
    match msg with
    | NoOp -> model, Cmd.none
    | Undo -> model, Cmd.none
    | Redo -> model, Cmd.none

// Own Logic
let render (model: Model) (dispatch: Msg -> unit) = 
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
                        svg.children [
                            Svg.rect [
                                svg.x 0
                                svg.y 0
                                svg.width model.canvasSize.width
                                svg.height model.canvasSize.height
                                svg.fill "white"
                                svg.stroke "black"
                                svg.strokeWidth 2
                                svg.rx 12
                                svg.ry 12
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]    
