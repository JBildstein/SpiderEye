@ECHO OFF
SETLOCAL

IF "%1"=="" (
    ECHO No NuGet API key provided
    EXIT /B 1
)

SET apiKey=%1
SET folder="Publish"

ECHO Prepare publish...
IF EXIST %folder% (
    rmdir %folder% /s /q
    IF %ERRORLEVEL% NEQ 0 (
      ECHO Removing folder %folder% failed
      EXIT /B 1
    )
)

ECHO Building project...
CALL dotnet pack Templates\SpiderEye.Templates -c Release -o %folder% > NUL
IF %ERRORLEVEL% NEQ 0 (
  ECHO Build failed!
  EXIT /B 1
)
ECHO Publishing package...
dotnet nuget push %folder%\*.nupkg -k %apiKey% -s https://api.nuget.org/v3/index.json
IF %ERRORLEVEL% NEQ 0 (
  ECHO Publish failed
  EXIT /B 1
)

ECHO Cleaning up...
rmdir %folder% /s /q

EXIT /B 0