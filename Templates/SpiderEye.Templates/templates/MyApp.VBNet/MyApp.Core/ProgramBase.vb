Imports SpiderEye

Public MustInherit Class ProgramBase
    Protected Shared Sub Run()
        Using window = New Window()
            window.Title = "MyApp"
            window.Icon = AppIcon.FromFile("icon", ".")

            Application.ContentProvider = New EmbeddedContentProvider("App")
            Application.Run(window, "index.html")
        End Using
    End Sub
End Class
