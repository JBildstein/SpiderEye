open System
open SpiderEye.Mac
open MyApp.Core

[<EntryPoint>]
let main argv =
    MacApplication.Init()
    mainBase()
