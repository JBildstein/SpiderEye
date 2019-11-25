# SpiderEye

Write .Net (Core) applications with a webview UI. It can be compared to how Electron runs on Node.js, SpiderEye runs on .Net.
Contrary to Electron though, SpiderEye uses the OS native webview instead of bundling Chromium.

What's the name supposed to mean? Simple: what kind of view does a spiders eye have? A webview! Get it? ...you'll laugh later :P

## Supported OS

| OS | Version | Runtime (minimum) | Webview | Browser Engine |
| ----- | ----- | ----- | ----- | ----- |
| Windows | 7, 8.x, 10 | .Net Core 3.0 or .Net 4.6.2 | WinForms WebBrowser control | IE 9-11 (depending on OS and installed version) |
| Windows |  10 Build 1803 or newer | .Net Core 3.0 or .Net 4.6.2 | WebViewControl | Edge |
| Linux | any x64 distro where .Net Core runs | .Net Core 2.0 | WebKit2GTK | WebKit |
| macOS | x64 10.13 or newer | .Net Core 2.0 | WKWebView | WebKit |

| Linux Dependencies | Used for | Optional |
| ----- | ----- | ----- |
| libgtk-3 | Application and window handling | No |
| libwebkit2gtk-4.0 | Webview | No |
| libappindicator3 | Status icon | Yes |

## Installation

| Type | Package Manager | Name | Version |
| ----- | ----- | ----- | ----- |
| Host (Windows) | NuGet | `Bildstein.SpiderEye.Windows` | [![NuGet](https://img.shields.io/nuget/v/Bildstein.SpiderEye.Windows.svg)](https://www.nuget.org/packages/Bildstein.SpiderEye.Windows/) |
| Host (Linux) | NuGet | `Bildstein.SpiderEye.Linux` | [![NuGet](https://img.shields.io/nuget/v/Bildstein.SpiderEye.Linux.svg)](https://www.nuget.org/packages/Bildstein.SpiderEye.Linux/) |
| Host (macOS) | NuGet | `Bildstein.SpiderEye.Mac` | [![NuGet](https://img.shields.io/nuget/v/Bildstein.SpiderEye.Mac.svg)](https://www.nuget.org/packages/Bildstein.SpiderEye.Mac/) |
| Project Templates | NuGet | `Bildstein.SpiderEye.Templates` | [![NuGet](https://img.shields.io/nuget/v/Bildstein.SpiderEye.Templates.svg)](https://www.nuget.org/packages/Bildstein.SpiderEye.Templates/) |
| Client | npm | `spidereye` | [![npm](https://img.shields.io/npm/v/spidereye.svg)](https://www.npmjs.com/package/spidereye) |

The client package is not required but you'll need it if you intend to communicate between host and webview.

## Getting Started

### Running the Examples

To get a quick overview of how a SpiderEye app looks like, have a look at the Example folder.
You can run them in various ways depending on what you use but you'll need .Net Core 3.0 for all of them.

**Visual Studio (Windows):** Open the solution, select the SpiderEye.Example.*.Windows project and set it as startup project and run it as you would any project.
When starting, you may get a "Just My Code" warning saying that you are trying to debug a Release build of SpiderEye.dll.
I believe this is a Visual Studio bug and can be safely ignored by selecting "Continue debugging" or "Continue debugging (don't ask again)".

**Visual Studio for Mac:** Same as with Visual Studio on Windows but you may get an error that .Net Core Desktop projects cannot be run on macOS.
Just right click on the Windows specific projects (in Examples/Simple, Examples/SPA and Playground) and select "Unload".

**Visual Studio Code:** Open the base folder (i.e. where the sln file lies), select which project you want to run in the Debug pane and hit start.
The launch/tasks.json is set up in a way that starts the project matching your current platform. Make sure that you have the C# extension installed before running.

**Console:** Simply go into the folder that matches your platform, e.g. Examples/Simple/App.Linux and call `dotnet run`

#### Simple Example

This is pretty much the simplest setup you can have.
There's a Core library that contains the web files and a project for each platform with a shared Program.cs

#### Single Page Application (SPA) Example

This is a slightly more advanced example using Angular for the web side of things.
It has the same project structure as the simple example but the Core project contains an angular app instead of static web files.

It is also set up in a way that uses the Angular dev server when running in Debug and uses the compiled Angular app (in the Angular/dist folder) when running in Release.
This means that you have to start the Angular dev server before you can run in Debug.
To do that, open a console window, go into the App.Core folder and call `npm run watch`.
On Windows 10, please also read [this note about Edge and localhost](#windows-10-edge-and-localhost) or you may see a blank page.

Before you can run/publish a Release build, you need to build the Angular app.
Same as with the dev server, open a console window, go into the App.Core folder and then call `npm run build:prod`.
On Windows you could also just call the publish.bat script in the Shared folder.
This script builds the Angular app and then publishes the application for each platform.

### Using the Templates

To really get started, it's easiest if you use the project templates. Install them like so:
```
dotnet new -i Bildstein.SpiderEye.Templates
```
Now that the templates are installed, create and go into the folder you want the projects in and call:
```
dotnet new spidereye-app
```
Relevant parameters for the template:

| Flag | Values | Default | Description |
| ----- | ----- | ----- | ----- |
| -lang, --language | C#, F#, VB | C# | Specifies the language of the template to create |
| -ns, --no-sln | true, false | false | Specifies if a solution file should not be added |
| -nv, --no-vscode | true, false | false | Specifies if the .vscode folder should not be added |

For more flags and more details, see the help output:
```
dotnet new spidereye-app --help
```

After `dotnet new` has run, everything is set up to get you started immediately.

If you use Visual Studio you can now open the sln or for Visual Studio Code, open the current directory.
Or you could try out things directly from the console by running the folder that matches your current operating system, e.g.:
```
dotnet run MyApp.Linux
```
The application should build, start and display a single window with "Hello World".

### Windows 10 Edge and localhost

The WebViewControl based on Edge does not allow localhost addresses for security reasons. More info [here](https://msdn.microsoft.com/en-us/library/windows/apps/hh780593.aspx).
To get around that restriction for development (e.g. to use the Angular dev server or similar), call this in the command line:
```
checknetisolation LoopbackExempt -a -n=Microsoft.Win32WebViewHost_cw5n1h2txyewy
```

**Note**: this is really ***only necessary for development***, a published app doesn't need it because everything is served from the embedded resources.

If you want to put that setting back to disallowing localhost, call the same command with the `-d` parameter:
```
checknetisolation LoopbackExempt -d -n=Microsoft.Win32WebViewHost_cw5n1h2txyewy
```

## Debug the Webview

Depending on which platform you are working, there are different ways to debug the webview.

### Windows Edge

For the Edge webview it's probably easiest if you use the [Microsoft Edge DevTools](https://www.microsoft.com/en-us/p/microsoft-edge-devtools-preview/9mzbfrmz0mnj)

While your app is running, open the Microsoft Edge DevTools (or hit refresh if it's already open) and click on the target that is your app.

### Windows IE

I haven't been able to get this to work but it should be possible to do it from Visual Studio by selecting Debug->Attach to Process.
In that dialog change the "Attach To" type by clicking on "Select" and there choose "Script".
Then select the process that is your app and click "Attach"

### Linux

First you need to set `WindowConfiguration.EnableDevTools` to `true` in your app.
Then just run your app and the dev tools will automatically attach to the window.
You can detach it to a separate window by clicking on the detach icon in the top left corner of the dev tools.

### macOS

First you need to set `WindowConfiguration.EnableDevTools` to `true` in your app.
Then run your app and once it's loaded, right click anywhere and select "Inspect Element" in the context menu.

## Development

To build the project you'll need an up-to-date version of Visual Studio 2019 or Visual Studio Code as well as the .Net Core SDK 3.0.
You can develop and run the project on all platforms but only if you target .Net Core.
Building for Windows requires .Net 4.6.2.
To run/build the SpiderEye.Client project, the SPA example or the Playground project, you also need node.js/npm.

## How it Works

For the window handling, this library calls the native APIs on Linux and macOS and the .Net APIs for Windows Forms on Windows.
It then does the same thing to attach the webview to that window.
On Windows, it also checks if the Edge based webview is available and falls back to the WebBrowser control if not.

Using the various APIs, the webview is set up to intercept requests and serve the files that are embedded in an assembly.
An exception is the WinForms WebBrowser control that doesn't support this and uses an internal localhost server instead.

The webview is also set up to inject a little bit of JavaScript at page load to add a consistent interface between the webview and .Net code.

## Contributing
Please check first if there is already an open issue or pull request before creating a new one.

#### Bugs
If there is an existing issue, upvote it or add more information there.
Otherwise create a new issue and make sure to fill out the template.

#### Feature Requests
If there is an existing issue, please upvote it.
Otherwise create a new issue and describe what you are expecting of the feature and why you need it.

#### Pull Requests
Make sure that you describe what the pull request is about and that you adhere to the StyleCop rules (no warnings should show up).
