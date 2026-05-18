module App.Alien
open System
open System.Threading
open App.Types
open App.Utils

let updateTick =
    createUpdateTick (fun s -> s.Tick) (fun t s -> {s with Tick = t})

let drawAlien state =
    match state.AlienState with
    | Alive -> displayMessage state.AlienX state.AlienY ConsoleColor.Green "🤖"
    | Hit -> displayMessage state.AlienX state.AlienY ConsoleColor.Red "💥"
    state



let drawMisiles state =
    state.Misiles
    |> List.iter (fun misil -> displayMessage misil.X misil.Y ConsoleColor.Cyan "=>")
    state

let drawGame = [|
    drawAlien
    drawMisiles
|]
let drawGameLoop = createRedrawScreen drawGame (fun s -> s.RedrawScreen) (fun s -> { s with RedrawScreen = false })