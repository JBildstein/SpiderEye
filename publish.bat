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

CALL :PackProject Windows
IF %ERRORLEVEL% NEQ 0 (
  ECHO Build failed!
  EXIT /B 1
)

CALL :PackProject Linux
IF %ERRORLEVEL% NEQ 0 (
  ECHO Build failed!
  EXIT /B 1
)

CALL :PackProject Mac
IF %ERRORLEVEL% NEQ 0 (
  ECHO Build failed!
  EXIT /B 1
)

ECHO Publishing package...
dotnet nuget push %folder%\*.nupkg --no-symbols true -k %apiKey% -s https://api.nuget.org/v3/index.json > NUL
IF %ERRORLEVEL% NEQ 0 (
  ECHO Publish failed
  EXIT /B 1
)

ECHO Cleaning up...
rmdir %folder% /s /q

EXIT /B 0

:PackProject
ECHO Building for %1...
dotnet pack Source\SpiderEye.%1 -c Release -o %folder% > NUL
EXIT /B
