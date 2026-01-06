module App

open Feliz
open Elmish
open Elmish.React
open Elmish.HMR // Elmish.HMR needs to be the last open instruction in order to be able to shadow any supported API
open Browser.Dom
 
   
let init,update,render = 
    PolygonDrawing.init, PolygonDrawing.update, PolygonDrawing.render
    //Canvas.init, Canvas.update, Canvas.render
    //Counter.init, Counter.update, Counter.render

console.log("Main.fs loaded - starting Elmish app")

Program.mkProgram init update render
|> Program.withReactSynchronous "feliz-app"
|> Program.run  
