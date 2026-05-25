module App.Types
open System


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

type SaveData = {
    Score: int
    Lives: int
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
    PlayerShootCooldown: int   
    EnemyShootCooldown:  int   
    EnemyRespawnTick:    int   
    Screen: MenuScreen
    Score: int
    Lives: int
}

let initialState = {
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
    PlayerShootCooldown = 0
    EnemyShootCooldown = 0
    EnemyRespawnTick = 0
    Screen = MainMenu
    Score = 0
    Lives = 3
}