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



let detectarColisionConEnemigo state =
    if state.EnemyState <> Alive then state  // ← si ya está Hit, no detectar
    else
        state.Misiles
        |> List.filter (fun misil -> not (misil.X = state.EnemyX-1 && misil.Y = state.EnemyY))
        |> fun nuevosMisiles ->
            if nuevosMisiles.Length <> state.Misiles.Length then 
                { state with 
                    EnemyState       = Hit
                    Misiles          = nuevosMisiles
                    RedrawScreen     = true
                    EnemyRespawnTick = 160
                    Score            = state.Score + 1 }
            else
                state
let drawEnemy state =
    match state.EnemyState with
    | Alive -> displayMessage state.EnemyX state.EnemyY ConsoleColor.Yellow "👾"
    | Hit -> displayMessage state.EnemyX state.EnemyY ConsoleColor.Red "💥"
    | Dead -> displayMessage state.EnemyX state.EnemyY ConsoleColor.DarkRed ""
    state


// Cambia esto en detectarColisionConPlayer:
let detectarColisionConPlayer state =
    state.MisilesEnemigos
    |> List.filter (fun misil -> not (misil.X = state.PlayerX+1 && misil.Y = state.PlayerY))
    |> fun nuevosMisiles ->
        if nuevosMisiles.Length <> state.MisilesEnemigos.Length then
            let nuevasVidas = state.Lives - 1
            { state with
                PlayerState       = Hit
                MisilesEnemigos   = nuevosMisiles
                Lives             = nuevasVidas
                PlayerRespawnTick = 80   // ← pausa antes de revivir
                Screen            = if nuevasVidas <= 0 then GameOverScreen else state.Screen
                RedrawScreen      = true }
        else
            state

// Y resetPlayer con el tick:
let resetPlayer state =
    if state.PlayerState = Hit && state.PlayerRespawnTick = 0 && state.Lives > 0 then
        { state with PlayerState = Alive; RedrawScreen = true }
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
        EnemyRespawnTick    = max 0 (state.EnemyRespawnTick - 1)
        PlayerRespawnTick   = max 0 (state.PlayerRespawnTick - 1) }  // ← nuevo

let procesarTecladoAlien key state =
    if state.PlayerState = Alive then 
        match key with
        | ConsoleKey.Escape -> { state with Screen = PauseMenu }  // ← nuevo
        | ConsoleKey.Spacebar ->
            if state.PlayerShootCooldown = 0 then
                let nuevoMisil = { X = state.PlayerX + 2; Y = state.PlayerY }
                { state with Misiles = nuevoMisil :: state.Misiles; PlayerShootCooldown = 8 }
            else state
        | ConsoleKey.UpArrow    -> { state with PlayerY = max 0 (state.PlayerY-1) }
        | ConsoleKey.DownArrow  -> { state with PlayerY = min (Console.BufferHeight-1) (state.PlayerY+1) }
        | ConsoleKey.LeftArrow  -> { state with PlayerX = max 0 (state.PlayerX-1) }
        | ConsoleKey.RightArrow -> { state with PlayerX = min (Console.BufferWidth-2) (state.PlayerX+1) }
        | _ -> state
        |> fun nuevoEstado ->
            if nuevoEstado <> state then { nuevoEstado with RedrawScreen = true }
            else state
    else
        state

let moverEnemigo state =
    if state.EnemyState = Alive && state.Tick % 3 = 0 then
        let nuevoY = state.EnemyY + state.EnemyDir
        let nuevoDir =
            if nuevoY <= 0 || nuevoY >= Console.BufferHeight - 1
            then -state.EnemyDir
            else state.EnemyDir
        { state with EnemyY = nuevoY; EnemyDir = nuevoDir; RedrawScreen = true }
    else
        state

let drawHUD state =
    displayMessage 0 0 ConsoleColor.White $"Score: {state.Score}   Lives: {state.Lives}"
    state
let processKeyboard =
    createProcessKeyboard (fun k state -> procesarTecladoAlien k.Key state)
let drawGame = [|
    drawAlien; drawEnemy; drawMisiles; drawMisilesEnemigos; drawHUD
|]
let drawGameLoop = createRedrawScreen drawGame (fun s -> s.RedrawScreen) (fun s -> { s with RedrawScreen = false })

let gamePipeline = [|
    updateTick                   // 1. avanzar el reloj
    processKeyboard              // 2. leer input del jugador
    descontarCooldowns
    moverEnemigo                 // 3. bajar timers (cooldown, respawn)
    actualizarMisiles            // 4. mover misiles del jugador
    actualizarMisilesEnemigos    // 5. mover misiles del enemigo
    actualizarDisparoEnemigo     // 6. el enemigo dispara si toca
    detectarColisionConEnemigo   // 7. ¿misil del jugador golpeó al enemigo?
    detectarColisionConPlayer                 // 8. ¿misil enemigo golpeó al jugador?
    resetEnemy  
    resetPlayer                  // 9. ¿el enemigo debe respawnear?
    drawGameLoop                 // 10. dibujar SOLO si hubo cambios
|]

let gameLoop =
    createMainLoop gamePipeline (fun s -> s.Screen = GameScreen)

