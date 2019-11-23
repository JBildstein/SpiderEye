Imports System.IO
Imports System.Reflection
Imports MyApp.Core
Imports SpiderEye
Imports SpiderEye.UI

Namespace MyApp
    Module Program
        <STAThread>
        Public Sub Main(args As String())
            Dim exeDir As String = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)

            Dim config = New WindowConfiguration() With {
                .Title = "MyApp",
                .ContentFolder = "App",
                .ContentAssembly = GetType(AssemblyMarker).Assembly,
                .Icon = AppIcon.FromFile("icon", exeDir)
            }

            Application.Run(config, "index.html")
        End Sub
    End Module
End Namespace
