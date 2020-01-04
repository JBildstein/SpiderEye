open System
open SpiderEye.Linux
open MyApp.Core

[<EntryPoint>]
let main argv =
    LinuxApplication.Init()
    mainBase()
