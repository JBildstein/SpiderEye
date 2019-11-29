open System
open SpiderEye.Windows
open fs.Core

[<EntryPoint>]
[<STAThread>]
let main argv =
    WindowsApplication.Init()
    mainBase()
