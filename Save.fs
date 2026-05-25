module App.Save
open System
open System.IO
open System.Text.Json
open App.Types

let private savePath = "save.json"

// State -> SaveData (extrae solo lo que se guarda)
let private toSaveData (state: State) : SaveData =
    { Score = state.Score; Lives = state.Lives }

// SaveData -> State (aplica los datos cargados al estado inicial)
let private applyToState (data: SaveData) (state: State) : State =
    { state with Score = data.Score; Lives = data.Lives }

// Guarda Score y Lives en save.json
let guardar (state: State) : unit =
    let data = toSaveData state
    let json = JsonSerializer.Serialize(data)
    File.WriteAllText(savePath, json)

// Carga save.json y devuelve Some(State) o None si no existe/falla
let cargar (state: State) : State option =
    if File.Exists(savePath) then
        try
            let json = File.ReadAllText(savePath)
            let data = JsonSerializer.Deserialize<SaveData>(json)
            Some (applyToState data state)
        with _ -> None
    else
        None

// ¿Existe un archivo guardado?
let existeGuardado () : bool =
    File.Exists(savePath)