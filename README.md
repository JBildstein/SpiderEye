# SpiderEye

Write .NET Core applications with a webview UI. It can be compared to how Electron runs on Node.js, SpiderEye runs on .NET.
Contrary to Electron though, SpiderEye uses the OS native webview instead of bundling Chromium.

What's the name supposed to mean? Simple: what kind of view does a spiders eye have? A webview! Get it? ...you'll laugh later :P

## Supported OS

| OS | Version | Runtime (minimum) | Webview | Browser Engine |
| ----- | ----- | ----- | ----- | ----- |
| Windows | 7, 8.x, 10 | .NET 5.0 | WinForms WebBrowser control | IE 9-11 (depending on OS and installed version) |
| Windows | 10 version 1809 or newer | .NET 5.0 | WebViewControl | Edge (MSHTML) |
| Windows | 7, 8.1, 10 | .NET 5.0 | WebView2 | Edge Chromium |
| Linux | any 64bit distro where .NET 5.0 runs | .NET 5.0 | WebKit2GTK | WebKit |
| macOS | x64 10.13 or newer | .NET 5.0 | WKWebView | WebKit |

| Linux Dependencies | Used for | Optional |
| ----- | ----- | ----- |
| libgtk-3 | Application and window handling | No |
| libwebkit2gtk-4.0 | Webview | No |
| libappindicator3 | Status icon | Yes |

### Edge (MSHTML)

To be able to use the MSHTML based Edge webview you'll have to add a windows version to the target framework moniker in your Windows project.
More specifically you'll have to open the MyApp.Windows project and change:
```xml
<TargetFramework>net5.0-windows</TargetFramework>
```
to
```xml
<TargetFramework>net5.0-windows10.0.17763</TargetFramework>
```
or any higher version.

Note that you'll have a wider OS support and a more modern browser engine if you go for the Edge Chromium webview, see the next point for more infos.

### Edge Chromium/WebView2

To use Edge Chromium/WebView2 you have to either install the WebView2 runtime or the Canary version of Edge Chromium.
Note that if you publish your app to your users they will also have to install it (follow the guidelines from Microsoft on that topic).

The fixed version distribution mode (also known as "bring your own") is currently not supported.

## Installation

| Type | Package Manager | Name | Version |
| ----- | ----- | ----- | ----- |
| Host (Common) | NuGet | `Bildstein.SpiderEye.Core` | [![NuGet](https://img.shields.io/nuget/v/Bildstein.SpiderEye.Core.svg)](https://www.nuget.org/packages/Bildstein.SpiderEye.Core/) |
| Host (Windows) | NuGet | `Bildstein.SpiderEye.Windows` | [![NuGet](https://img.shields.io/nuget/v/Bildstein.SpiderEye.Windows.svg)](https://www.nuget.org/packages/Bildstein.SpiderEye.Windows/) |
| Host (Linux) | NuGet | `Bildstein.SpiderEye.Linux` | [![NuGet](https://img.shields.io/nuget/v/Bildstein.SpiderEye.Linux.svg)](https://www.nuget.org/packages/Bildstein.SpiderEye.Linux/) |
| Host (macOS) | NuGet | `Bildstein.SpiderEye.Mac` | [![NuGet](https://img.shields.io/nuget/v/Bildstein.SpiderEye.Mac.svg)](https://www.nuget.org/packages/Bildstein.SpiderEye.Mac/) |
| Project Templates | NuGet | `Bildstein.SpiderEye.Templates` | [![NuGet](https://img.shields.io/nuget/v/Bildstein.SpiderEye.Templates.svg)](https://www.nuget.org/packages/Bildstein.SpiderEye.Templates/) |
| Client | npm | `spidereye` | [![npm](https://img.shields.io/npm/v/spidereye.svg)](https://www.npmjs.com/package/spidereye) |

The client package is not required but you'll need it if you intend to communicate between host and webview.

## Getting Started

### Running the Examples

To get a quick overview of how a SpiderEye app looks like, have a look at the Example folder.

**Visual Studio (Windows):** Open the solution, select the SpiderEye.Example.*.Windows project and set it as startup project and run it as you would any project.
When starting, you may get a "Just My Code" warning saying that you are trying to debug a Release build of SpiderEye.dll.
I believe this is a Visual Studio bug and can be safely ignored by selecting "Continue debugging" or "Continue debugging (don't ask again)".

**Visual Studio for Mac:** Same as with Visual Studio on Windows but you may get an error that .NET Core Desktop projects cannot be run on macOS.
Just right click on the Windows specific projects and select "Unload".

**Visual Studio Code:** Open the base folder (i.e. where the SpiderEye.sln file lies), select which project you want to run in the Debug pane and hit start.
The launch/tasks.json is set up in a way that starts the project matching your current platform. Make sure that you have the C# extension installed before running.

**Console:** Simply go into the folder that matches your platform, e.g. `Examples/Simple/App.Linux` and call `dotnet run`

#### Simple Example

This is pretty much the simplest setup you can have.
There are projects for each platform and there's a Core library that contains the web files and common startup logic.

#### Single Page Application (SPA) Example

This is a slightly more advanced example using Angular for the web side of things.
It has the same project structure as the simple example but the Core project contains an Angular app instead of static web files.

It is also set up in a way that uses the Angular dev server when running in Debug and uses the compiled Angular app (in the `Angular/dist` folder) when running in Release.
This means that you have to start the Angular dev server before you can run in Debug.
To do that, open a console window and `cd` into the App.Core folder. First install the packages with `npm i` and then call `npm run watch`.
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
| -ns, --no-sln | true, false | false | Specifies if the solution file should not be added |
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

### Windows Edge Chromium

First you need to set `Window.EnableDevTools` to `true` in your app.
Then just run your app and the dev tools will automatically open in a new window.

### Windows Edge (MSHTML)

For the Edge webview it's probably easiest if you use the [Microsoft Edge DevTools](https://www.microsoft.com/en-us/p/microsoft-edge-devtools-preview/9mzbfrmz0mnj)

While your app is running, open the Microsoft Edge DevTools (or hit refresh if it's already open) and click on the target that is your app.

### Windows IE

I haven't been able to get this to work but it should be possible to do it from Visual Studio by selecting Debug->Attach to Process.
In that dialog change the "Attach To" type by clicking on "Select" and there choose "Script".
Then select the process that is your app and click "Attach"

### Linux

First you need to set `Window.EnableDevTools` to `true` in your app.
Then just run your app and the dev tools will automatically attach to the window.
You can detach it to a separate window by clicking on the detach icon in the top left corner of the dev tools.

### macOS

First you need to set `Window.EnableDevTools` to `true` in your app.
Then run your app and once it's loaded, right click anywhere and select "Inspect Element" in the context menu.

## Publishing your App

You can publish your application like any other .NET Core app by calling `dotnet publish` with the appropriate runtime identifier, e.g. for Linux:
```
dotnet publish -r linux-x64
```
Or alternatively use whatever mechanism your IDE (like Visual Studio) provides.
In the `Examples/**/Shared` folders you'll also find build scripts that include all platforms and should provide a more complete insight.

For macOS you'll likely want a proper app instead of an executable with a bunch of files. It's pretty easy to do.
First you need an `Info.plist` file like you'd have for any other macOS app. Here's an example to get you started:
```
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleName</key>
    <string>MyApp</string>
    <key>CFBundleIconFile</key>
    <string>MyApp.icns</string>
    <key>CFBundleIdentifier</key>
    <string>com.mycompany.myapp</string>
    <key>CFBundleShortVersionString</key>
    <string>1.0.0</string>
    <key>LSMinimumSystemVersion</key>
    <string>10.13</string>
    <key>CFBundleInfoDictionaryVersion</key>
    <string>6.0</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
</dict>
</plist>
```
You can find more information in the [Apple documentation](https://developer.apple.com/documentation/bundleresources/information_property_list).

Then create a folder structure like so (MyApp.app is the root folder):
```
MyApp.app/
 └─Contents/
    │  └─Info.plist
    │
    ├─MacOS/
    │  ├─MyApp
    │  ├─MyApp.dll
    │  └─etc...
    │
    └─Resources/
       └─MyApp.icns
```
In the MacOS folder you copy everything that was created from the publish, i.e. your executable and all the DLLs etc.
In the Resources folder you put your application icon with the same name as you have defined in the Info.plist.

## Development

To build the project you'll need an up-to-date version of Visual Studio 2019 or Visual Studio Code as well as the .NET Core SDK 5.0.
To run/build the SpiderEye.Client project, the SPA example or the Playground project, you also need node.js/npm.

Before running the Playground project, make sure that you have all client side packages installed and the Angular dev server running.
Client side packages need to be installed with `npm i` in the `Source/SpiderEye.Client` folder and the `Playground/SpiderEye.Playground.Core` folder.
Once those are installed, call `npm run watch` in the `Playground/SpiderEye.Playground.Core` folder.

## How it Works

For the window handling, this library calls the native APIs on Linux and macOS and the .NET APIs for Windows Forms on Windows.
It then does the same thing to attach the webview to that window.
On Windows, it also checks if the Edge (MSHTML) or Edge Chromium based webview is available and falls back to the WebBrowser control if not.

Using the various APIs, the webview is set up to intercept requests and serve the files that are embedded in an assembly.
An exception is the WinForms WebBrowser control and Edge Chromium webview that don't support this and uses an internal localhost server instead.

The webview is also set up to inject a little bit of JavaScript at page load to add a consistent interface between the webview and .NET code.

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
