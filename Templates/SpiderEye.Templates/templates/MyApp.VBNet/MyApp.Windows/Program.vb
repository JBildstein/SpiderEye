Imports SpiderEye.Windows
Imports MyApp.Core

Class Program
    Inherits ProgramBase

    <STAThread>
    Public Shared Sub Main(ByVal args As String())
        WindowsApplication.Init()
        Run()
    End Sub
End Class
