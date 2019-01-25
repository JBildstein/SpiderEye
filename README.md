# SpiderEye

Write .Net (Core) applications with a webview UI. It can be compared to how Electron runs on Node.js, SpiderEye runs on .Net.
Contrary to Electron though, SpiderEye uses the OS native webview instead of bundling Chrome.

What's the name supposed to mean? Simple: what kind of view does a spiders eye have? A webview! Get it? ...you'll laugh later :P

## Supported OS

| OS | Version | Webview | Browser Engine |
| ----- | ----- | ----- | ----- |
| Windows | 7, 8.x, 10 | WPF WebBrowser control | IE 9-11 (depending on OS and installed version) |
| Windows |  10 Build 1803 | WebViewControl | Edge |
| Linux | libwebkit2gtk-4.0 libgtk-3 | WebKit2GTK | WebKit |
| macOS | 10.13+ x64 | WKWebView | WebKit |

## Installation

Use NuGet to install the SpiderEye package: `Bildstein.SpiderEye` [![NuGet](https://img.shields.io/nuget/v/Bildstein.SpiderEye.svg)](https://www.nuget.org/packages/Bildstein.SpiderEye/)

## Getting Started

A SpiderEye app can be created from a normal .Net Core console app. Only a few small adjustments to the project file are needed to make it work well:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <!-- On Windows, this is necessary to hide the console window. On other platforms it falls back to Exe -->
    <OutputType>WinExe</OutputType>
    <!-- For Windows we need full .Net 4.6.2+, for Linux and macOS .Net Core 2.x -->
    <TargetFrameworks>net462;netcoreapp2.2</TargetFrameworks>
    <!-- This is needed if you wan to create a standalone app -->
    <RuntimeIdentifiers>win;linux;osx</RuntimeIdentifiers>
  </PropertyGroup>

  <ItemGroup>
    <!-- Reference to the SpiderEye NuGet package -->
    <PackageReference Include="Bildstein.SpiderEye" Version="1.0.0-alpha.3" />
  </ItemGroup>

  <ItemGroup>
    <!-- The App folder is where all our html, css, js, etc. files are (change if you use a different folder) -->
    <EmbeddedResource Include="App\**">
      <!-- this retains the original filename of the embedded files (required to located them later) -->
      <LogicalName>%(RelativeDir)%(Filename)%(Extension)</LogicalName>
    </EmbeddedResource>
  </ItemGroup>
</Project>
```

The actual program now is just a matter of a bit of configuration and then starting it:

```c-sharp
using System;

namespace SpiderEyeExample
{
    class Program
    {
        // STAThread is required for WPF
        [STAThread]
        public static void Main(string[] args)
        {
            // this creates a new app configuration with default values
            var config = new AppConfiguration();

            // this relates to the path defined in the .csproj file
            config.ContentFolder = "App";

            // run the application
            Application.Run(config);
        }
    }
}
```

To make sure that the newest IE version is used on Windows, set the `doctype` and add the `X-UA-Compatible` header with the value `IE=edge`:
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

Then to publish that project, you can use the dotnet CLI like this (assuming a standalone publish):\
Windows: `dotnet publish -c Release -f net462 -r win -o ./bin/Publish/Windows`\
Linux: `dotnet publish -c Release -f netcoreapp2.2 -r linux-x64 -o ./bin/Publish/Linux`\
macOS: `dotnet publish -c Release -f netcoreapp2.2 -r osx-64 -o ./bin/Publish/Mac`

Further examples can be found in the `Examples` folder

#### Windows 10 Edge and localhost

The WebViewControl based on Edge does not allow localhost addresses for security reasons. More info [here](https://msdn.microsoft.com/en-us/library/windows/apps/hh780593.aspx).
To get around that restriction for development (e.g. to use the Angular dev server or similar), call this in the command line:\
`checknetisolation LoopbackExempt -a -n=Microsoft.Win32WebViewHost_cw5n1h2txyewy`

## Development

To build the project you'll need an up-to-date version of Visual Studio 2017 or Visual Studio Code as well as the .Net Core SDK 2.2.
You can develop and run the project on all platforms but only if you target .Net Core/Standard.
Building for Windows requires .Net 4.6.2, WPF and the Windows 10 SDK.

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
