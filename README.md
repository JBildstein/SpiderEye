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
| Client | npm | `spidereye` | [![npm](https://img.shields.io/npm/v/spidereye.svg)](https://www.npmjs.com/package/spidereye) |

The client package is not required but you'll need it if you intend to communicate between host and webview.

## Getting Started

To try things out, best head over to the Examples folder and have a look at those. The basic requirements for a new app are outlined here:

A SpiderEye app can be created from a normal .Net Core console app (Linux, macOS) or a .Net Core (SDK style) Windows Forms app (Windows).
You need a separate project for each platform you want to use because you need to reference a different NuGet package for each platform
and for any bigger app it's likely that you'll have some platform specific code anyway.

Now, to have your app load the client side files, add the following snippet to your project file(s):

```xml
<ItemGroup>
<!-- The App folder is where all our html, css, js, etc. files are (change if you use a different folder) -->
<EmbeddedResource Include="App\**">
    <!-- this retains the original filename of the embedded files (required to located them later) -->
    <LogicalName>%(RelativeDir)%(Filename)%(Extension)</LogicalName>
</EmbeddedResource>
</ItemGroup>
```

The actual program is just a matter of a bit of configuration and then starting it:

```c-sharp
using System;
using SpiderEye;
using SpiderEye.UI;

namespace SpiderEyeExample
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // this creates a new app configuration with default values
            var config = new WindowConfiguration();

            // this relates to the path defined in the .csproj file
            config.ContentFolder = "App";

            // runs the application and opens a window with the given page loaded
            Application.Run(config, "index.html");
        }
    }
}
```

To make sure that the newest available IE version is used on Windows, set the `doctype` and add the `X-UA-Compatible` header with the value `IE=edge`:
```html
<!doctype html>
<html lang="en">
<head>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />

    <!-- Other stuff you may want to add -->

    <title>Hello World</title>

    <link href="site.css" rel="stylesheet" />
</head>
<body>
    <h1>Hello World</h1>

    <script src="site.js" type="text/javascript"></script>
</body>
</html>
```

The Folder structure for this example looks like this:
```
SpiderEyeExample/
├─ App/
│ ├─ index.html
│ └─ (other .css, .js, etc. files and subfolders)
├─ Program.cs
└─ SpiderEyeExample.csproj (or vbproj, fsproj etc)
```
**Note:** This folder structure is just for a single platform. If you want other platforms,
 add another project per platform and set up something for sharing the client code.
Have a look at the examples for different ways to do that. Simply put, you could put it into a folder
outside of the projects and adjust the paths for embedding or you could use another project (normal library)
that has the files embedded and then set `WindowConfiguration.ContentAssembly` to use it.

Then to publish that project, you can use the dotnet CLI like this (assuming a standalone publish):\
Windows: `dotnet publish -c Release -f netcoreapp3.0 -r win-x64 -o ./bin/Publish/Windows`\
Linux: `dotnet publish -c Release -f netcoreapp3.0 -r linux-x64 -o ./bin/Publish/Linux`\
macOS: `dotnet publish -c Release -f netcoreapp3.0 -r osx.10.13-x64 -o ./bin/Publish/Mac`

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
