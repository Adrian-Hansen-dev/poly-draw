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
        { canvasSize = { width = 800.0; height = 600.0 } }
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
        prop.style [ style.padding 20; style.fontFamily "Times New Roman, sans-serif" ]
        prop.children [
            Html.h1 "Custom Polygon Sketching App"
            Html.button [
                prop.style [style.margin 20]; 
                prop.onClick (fun _ -> dispatch Undo)
                prop.text "Undo"
            ]
            Html.button [
                prop.style [style.margin 20]; 
                prop.onClick (fun _ -> dispatch Redo)
                prop.text "Redo"
            ]
            // ... rest of the rendering logic
        ]
    ]    
