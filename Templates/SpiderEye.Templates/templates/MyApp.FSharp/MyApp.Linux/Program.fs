open System
open SpiderEye.Linux
open MyApp.Core

[<EntryPoint>]
[<STAThread>]
let main argv =
    LinuxApplication.Init()
    mainBase()
