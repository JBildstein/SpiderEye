open System
open System.IO
open System.Reflection
open SpiderEye
open SpiderEye.UI
open MyApp

[<EntryPoint>]
[<STAThread>]
let main argv =
    let exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
    let config = WindowConfiguration(Title = "MyApp",
                                    ContentFolder = "App",
                                    ContentAssembly = typedefof<AssemblyMarker>.Assembly,
                                    Icon = AppIcon.FromFile("icon", exeDir))
    Application.Run(config, "index.html")
    0
