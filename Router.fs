module App.Router
open System
open App.Types
open App.Utils
open App.Menu
let route (state: State) : State =
    match state.Screen with
    | MainMenu ->
        let comando = App.Menu.mostrar()
        match comando with
        | NewGame  -> { state with Screen = GameScreen }
        | LoadGame -> state  // por ahora placeholder
        | Exit     -> Environment.Exit(0); state

    | GameScreen     -> state  // placeholder
    | PauseMenu      -> state  // placeholder
    | GameOverScreen -> state  // placeholder