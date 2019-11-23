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

To get a quick overview of how a SpiderEye app looks like, have a look at the Example folder.

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
| ----- | ----- | ----- | ----- | ----- |
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

#### Windows 10 Edge and localhost

The WebViewControl based on Edge does not allow localhost addresses for security reasons. More info [here](https://msdn.microsoft.com/en-us/library/windows/apps/hh780593.aspx).
To get around that restriction for development (e.g. to use the Angular dev server or similar), call this in the command line:\
`checknetisolation LoopbackExempt -a -n=Microsoft.Win32WebViewHost_cw5n1h2txyewy`

**Note**: this is really only for development, a published app doesn't need it because everything is served from the embedded resources.

## Development

To build the project you'll need an up-to-date version of Visual Studio 2019 or Visual Studio Code as well as the .Net Core SDK 3.0.
You can develop and run the project on all platforms but only if you target .Net Core.
Building for Windows requires .Net 4.6.2.

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
