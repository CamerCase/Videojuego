module App.Enemy
open System
open App.Types
open App.Utils
open  System.Threading

let drawEnemy state =
    match state.EnemyState with
    | Alive -> displayMessage state.EnemyX state.EnemyY ConsoleColor.Yellow "👾"
    | Hit -> displayMessage state.EnemyX state.EnemyY ConsoleColor.Red "💥"
    state

let drawMisilesEnemigos state =
    state.MisilesEnemigos
    |> List.iter (fun misil -> displayMessage misil.X misil.Y ConsoleColor.Red "<=")
    state

