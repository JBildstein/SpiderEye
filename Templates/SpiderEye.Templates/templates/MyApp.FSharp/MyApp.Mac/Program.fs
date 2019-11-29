open System
open SpiderEye.Mac
open MyApp.Core

[<EntryPoint>]
[<STAThread>]
let main argv =
    MacApplication.Init()
    mainBase()
