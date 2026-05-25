module App.Program
open System 
open App.Types
open App.Utils
open App.Menu

let estadoPrueba = { initialState with Score = 5; Lives = 2 }
App.Save.guardar estadoPrueba
printfn "Guardado OK"


match App.Save.cargar initialState with
| Some s -> printfn "Cargado: Score=%d Lives=%d" s.Score s.Lives
| None   -> printfn "No hay guardado"

mostrar() |> ignore