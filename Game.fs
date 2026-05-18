module App.Game
open System
open System.Threading
open App.Types
open App.Utils

let drawAlien state =
    match state.AlienState with
    | Alive -> displayMessage state.AlienX state.AlienY ConsoleColor.Green "🤖"
    | Hit -> displayMessage state.AlienX state.AlienY ConsoleColor.Red "💥"
    state

let drawEnemy state =
    match state.EnemyState with
    | Alive -> displayMessage state.EnemyX state.EnemyY ConsoleColor.Yellow "👾"
    | Hit -> displayMessage state.EnemyX state.EnemyY ConsoleColor.Red "💥"
    state

