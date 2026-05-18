module App.Types
open System

type ProgramState =
| Running
| Terminated

type SpriteState =
| Alive
| Hit

type Misil = {
    X: int
    Y: int
}


type State = {
    ProgramState: ProgramState
    AlienX: int
    AlienY: int
    AlienState: SpriteState
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
}

let estadoInicial = {
    ProgramState = Running
    AlienX = Console.BufferWidth/2
    AlienY = Console.BufferHeight/2
    AlienState = Alive
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
}

