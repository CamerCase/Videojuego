module App.Save
open System
open System.IO
open System.Text.Json
open App.Types


let private savePath = "save.json"
let borrar () : unit =
    if File.Exists(savePath) then
        File.Delete(savePath)

let private toSaveData (state: State) : SaveData =
    { Score = state.Score; Lives = state.Lives }


let private applyToState (data: SaveData) (state: State) : State =
    { state with Score = data.Score; Lives = data.Lives }


let guardar (state: State) : unit =
    let data = toSaveData state
    let json = JsonSerializer.Serialize(data)
    File.WriteAllText(savePath, json)

let cargar (state: State) : State option =
    if File.Exists(savePath) then
        let json = File.ReadAllText(savePath)
        JsonSerializer.Deserialize<SaveData>(json)
        |> fun data -> Some (applyToState data state)
    else
        None

let existeGuardado () : bool =
    File.Exists(savePath)