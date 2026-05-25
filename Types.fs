module App.Types
open System

type ProgramState =
| Running
| Terminated

type SpriteState =
| Alive
| Hit
| Dead

type Menu =
| Active
| Inactive

type MenuScreen =
| MainMenu
| GameScreen
| PauseMenu
| GameOverScreen

type Misil = {
    X: int
    Y: int
}


type Command =
| NewGame
| LoadGame
| Exit


type MenuState = {
    Menu: Menu
    X: int; Y: int
    CurSorSelection: int
    CursorX: int
    Commands: (Command * string) array
    RedrawScreen: bool
}

let initialMenuState = {
    Menu = Active
    X = Console.BufferWidth/2 - 5
    Y = Console.BufferHeight/2 - 1
    CurSorSelection = 0
    CursorX = Console.BufferWidth/2 - 7
    Commands = [| (NewGame, "New Game"); (LoadGame, "Load Game"); (Exit, "Exit") |]
    RedrawScreen = true
}

type State = {
    ProgramState: ProgramState
    PlayerX: int
    PlayerY: int
    PlayerState: SpriteState
    RedrawScreen: bool
    Tick: int
    Misiles: Misil list
    EnemyX: int
    EnemyY: int
    EnemyDir: int
    EnemyState: SpriteState
    MisilesEnemigos: Misil list
    ColisionAlien: int
    ColisionEnemigo: int
    Lives : int
    Score : int
    Screen: MenuScreen
}

let initialState = {
    ProgramState = Running
    PlayerX = Console.BufferWidth/2
    PlayerY = Console.BufferHeight/2
    PlayerState = Alive
    RedrawScreen = true
    Tick = -1
    Misiles = []
    EnemyX = Console.BufferWidth-2
    EnemyY = 0
    EnemyDir = 1
    EnemyState = Alive
    MisilesEnemigos = []
    ColisionAlien = 0
    ColisionEnemigo = 0
    Lives = 3
    Score = 0
    Screen = MainMenu
}