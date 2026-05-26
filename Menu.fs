module App.Menu
open System
open App.Types
open App.Utils

let drawMenu state =
    state.Commands
    |> Array.iteri (fun i (_,mensaje) ->
    displayMessage state.X (state.Y+i) ConsoleColor.Cyan mensaje)
    displayMessage state.CursorX (state.Y+state.CurSorSelection) ConsoleColor.Yellow "*"
    state 

let updateMenuKeyboard key state =
    let newState =
        match key with 
        | ConsoleKey.UpArrow   -> {state with CurSorSelection = max 0 (state.CurSorSelection-1)}
        | ConsoleKey.DownArrow -> {state with CurSorSelection = min (state.Commands.Length-1) (state.CurSorSelection+1)}
        | ConsoleKey.Enter     -> {state with Menu = Inactive}
        | _ -> state
    if newState <> state then {newState with RedrawScreen = true}
    else state

let processKeyboard =
    createProcessKeyboard (fun k state -> updateMenuKeyboard k.Key state)

let drawMenuLoop = createRedrawScreen [| drawMenu |] (fun s -> s.RedrawScreen) (fun s -> { s with RedrawScreen = false })



let pipeline = [| processKeyboard; drawMenuLoop |]

let miLoop = createMainLoop pipeline (fun s -> s.Menu = Active)

let mostrar() =
    let oldForeground = System.Console.ForegroundColor
    System.Console.CursorVisible <- false
    let state = initialMenuState |> miLoop
    System.Console.CursorVisible <- true
    System.Console.ForegroundColor <- oldForeground
    System.Console.Clear()
    state.Commands.[state.CurSorSelection] |> fst

let mostrarGameOver score =
    Console.Clear()
    Console.CursorVisible <- false

    // Estado mínimo: solo necesita saber si sigue activo
    let state = { 
        initialMenuState with 
            Commands = [| (NewGame, "Volver al menu") |]
            CurSorSelection = 0
    }

    // Dibuja el mensaje fijo
    displayMessage (Console.BufferWidth/2 - 5) (Console.BufferHeight/2 - 2) ConsoleColor.Red   "GAME OVER"
    displayMessage (Console.BufferWidth/2 - 5) (Console.BufferHeight/2 - 1) ConsoleColor.White $"Score: {score}"

    // Reutiliza el mismo loop del menú
    let resultado = state |> miLoop
    Console.Clear()
    resultado.Commands.[0] |> fst

let mostrarPausa () =
    Console.CursorVisible <- false
    let state = {
        initialMenuState with
            Commands = [| 
                (NewGame, "Continuar")   // reutilizamos NewGame como "continuar"
                (Exit,    "Guardar y salir al menu") 
            |]
            CurSorSelection = 0
            RedrawScreen = true
    }
    let resultado = state |> miLoop
    Console.Clear()
    resultado.Commands.[resultado.CurSorSelection] |> fst