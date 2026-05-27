module App.Router
open System
open App.Types
open App.Utils
let route (state: State) : State =
    match state.Screen with
    | MainMenu ->
        let comando = App.Menu.mostrar()
        match comando with
        | NewGame  -> { initialState with Screen = GameScreen }
        | LoadGame ->
            match App.Save.cargar initialState with
            | Some s -> { s with Screen = GameScreen }
            | None   -> { initialState with Screen = GameScreen }
        | Exit     -> Environment.Exit(0); state

    | GameScreen ->
        Console.CursorVisible <- false
        let result = state |> App.Game.gameLoop
        Console.CursorVisible <- true
        result                                    

    | GameOverScreen ->
        App.Save.borrar()                              
        App.Menu.mostrarGameOver state.Score |> ignore
        { initialState with Screen = MainMenu }

    | PauseMenu ->
        match App.Menu.mostrarPausa() with
        | NewGame -> { state with Screen = GameScreen; RedrawScreen = true }  
        | Exit    ->
            App.Save.guardar state
            { initialState with Screen = MainMenu }
        | _ -> state
let rec routerLoop state =
    state |> route |> routerLoop