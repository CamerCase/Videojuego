module App.Game
open System 
open System.Threading
open App.Types
open App.Utils

let updateTick =
    createUpdateTick (fun s -> s.Tick) (fun t s -> {s with Tick = t})

let drawAlien state =
    match state.PlayerState with
    | Alive -> displayMessage state.PlayerX state.PlayerY ConsoleColor.Green "🤖"
    | Hit -> displayMessage state.PlayerX state.PlayerY ConsoleColor.Red "💥"
    | Dead -> displayMessage state.PlayerX state.PlayerY ConsoleColor.DarkRed ""
    state
let drawMisiles state =
    state.Misiles
    |> List.iter (fun misil -> displayMessage misil.X misil.Y ConsoleColor.Cyan "=>")
    state

let drawEnemy state =
    match state.EnemyState with
    | Alive -> displayMessage state.EnemyX state.EnemyY ConsoleColor.Yellow "👾"
    | Hit -> displayMessage state.EnemyX state.EnemyY ConsoleColor.Red "💥"
    | Dead -> displayMessage state.EnemyX state.EnemyY ConsoleColor.DarkRed ""
    state

let drawMisilesEnemigos state =
    state.MisilesEnemigos
    |> List.iter (fun misil -> displayMessage misil.X misil.Y ConsoleColor.Red "<=")
    state


let procesarTecladoAlien key state =
    if state.PlayerState = Alive then 
        match key with 
        | ConsoleKey.Spacebar ->
            if state.PlayerShootCooldown = 0 then  // ← agregar esto
                let nuevoMisil = { X = state.PlayerX + 2; Y = state.PlayerY }
                { state with Misiles = nuevoMisil :: state.Misiles;PlayerShootCooldown = 8 }  // ← y esto
            else
                state
        | ConsoleKey.UpArrow ->
            {state with PlayerY = max 0 (state.PlayerY-1)}
        | ConsoleKey.DownArrow ->
            {state with PlayerY = min (Console.BufferHeight-1) (state.PlayerY+1)}
        | ConsoleKey.LeftArrow ->
            {state with PlayerX = max 0 (state.PlayerX-1)}
        | ConsoleKey.RightArrow ->
            {state with PlayerX = min (Console.BufferWidth-2) (state.PlayerX+1)}
        | _ -> state
        |> fun nuevoEstado ->
            if nuevoEstado <> state then 
                {nuevoEstado with RedrawScreen=true}
            else
                state
    else
        state


let processKeyboard =
    createProcessKeyboard (fun k state -> procesarTecladoAlien k.Key state)
let drawGame = [|
    drawAlien
    drawMisiles
    drawEnemy
    drawMisilesEnemigos
|]
let drawGameLoop = createRedrawScreen drawGame (fun s -> s.RedrawScreen) (fun s -> { s with RedrawScreen = false })