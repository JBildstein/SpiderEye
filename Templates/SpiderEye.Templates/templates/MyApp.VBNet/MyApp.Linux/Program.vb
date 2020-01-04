Imports SpiderEye.Linux
Imports MyApp.Core

Class Program
    Inherits ProgramBase

    Public Shared Sub Main(ByVal args As String())
        LinuxApplication.Init()
        Run()
    End Sub
End Class
