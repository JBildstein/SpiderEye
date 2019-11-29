module MyApp.Core
open SpiderEye

let mainBase() =
    use window = new Window()
    window.Title <- "MyApp"
    window.Icon <- AppIcon.FromFile("icon", ".")

    Application.ContentProvider <- EmbeddedContentProvider("App")
    Application.Run(window, "index.html")
    0
