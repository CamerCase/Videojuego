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

let actualizarMisiles state =
    if state.Misiles <> [] then 
        state.Misiles
        |> Seq.map (fun misil -> {misil with X=misil.X+1})
        |> Seq.filter (fun misil -> misil.X < Console.BufferWidth-2)
        |> Seq.toList
        |> fun nuevosMisiles ->
            {state with Misiles = nuevosMisiles;RedrawScreen=true} 
    else
        state



let detectarColisionConPlayer state =
    state.MisilesEnemigos
    |> List.filter (fun misil -> not (misil.X = state.PlayerX+1 && misil.Y = state.PlayerY))
    |> fun nuevosMisiles ->
        if nuevosMisiles.Length <> state.MisilesEnemigos.Length then 
            {state with 
                PlayerState = Hit
                MisilesEnemigos = nuevosMisiles
                Lives = state.Lives - 1
                Screen = if state.Lives - 1 <= 0 then GameOverScreen else state.Screen
                RedrawScreen = true 
            }
        else
            state
let drawEnemy state =
    match state.EnemyState with
    | Alive -> displayMessage state.EnemyX state.EnemyY ConsoleColor.Yellow "👾"
    | Hit -> displayMessage state.EnemyX state.EnemyY ConsoleColor.Red "💥"
    | Dead -> displayMessage state.EnemyX state.EnemyY ConsoleColor.DarkRed ""
    state


let detectarColisionConEnemigo state =
    state.Misiles
    |> List.filter (fun misil -> not (misil.X = state.EnemyX-1 && misil.Y = state.EnemyY))
    |> fun nuevosMisiles ->
        if nuevosMisiles.Length <> state.Misiles.Length then 
            {state with 
                EnemyState=Hit
                Misiles=nuevosMisiles
                RedrawScreen=true
                EnemyRespawnTick = 160          // cuenta regresiva de 160 ticks
                Score = state.Score + 1   
            }
        else
            state
let resetEnemy state =
    if state.EnemyState = Hit && state.EnemyRespawnTick = 0 then
        { state with EnemyState = Alive; RedrawScreen = true }
    else
        state

let drawMisilesEnemigos state =
    state.MisilesEnemigos
    |> List.iter (fun misil -> displayMessage misil.X misil.Y ConsoleColor.Red "<=")
    state

let actualizarMisilesEnemigos state =
    if state.MisilesEnemigos <> [] then 
        state.MisilesEnemigos
        |> Seq.map (fun misil -> {misil with X=misil.X-1})
        |> Seq.filter (fun misil -> misil.X >= 0)
        |> Seq.toList
        |> fun nuevosMisiles ->
            {state with MisilesEnemigos = nuevosMisiles;RedrawScreen=true} 
    else
        state

let actualizarDisparoEnemigo state =
    if state.EnemyState = Alive && state.Tick % 10 = 0 then 
        let nuevoMisil = {
            X = state.EnemyX-2
            Y = state.EnemyY
        }
        {state with MisilesEnemigos= nuevoMisil :: state.MisilesEnemigos; RedrawScreen=true}
    else
        state

let descontarCooldowns state =
    { state with
        PlayerShootCooldown = max 0 (state.PlayerShootCooldown - 1)
        EnemyRespawnTick    = max 0 (state.EnemyRespawnTick - 1) }

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