@ECHO OFF
SETLOCAL

CD ..\Client
ECHO Linting files...
CALL npm run lint > NUL
IF %ERRORLEVEL% NEQ 0 (
  ECHO Linting failed
  EXIT /B 1
)

ECHO Building angular...
CALL npm run build:prod > NUL
IF %ERRORLEVEL% NEQ 0 (
  ECHO Angular build failed
  EXIT /B 1
)

CD ..
CALL :PublishProject Windows win-x64
IF %ERRORLEVEL% NEQ 0 (
  ECHO Publish failed
  EXIT /B 1
)

CALL :PublishProject Linux linux-x64
IF %ERRORLEVEL% NEQ 0 (
  ECHO Publish failed
  EXIT /B 1
)

CALL :PublishProject Mac osx-x64
IF %ERRORLEVEL% NEQ 0 (
  ECHO Publish failed
  EXIT /B 1
)

ECHO Done!

EXIT /B 0

:PublishProject
ECHO Publishing for %1...
CALL dotnet publish App.%1\SpiderEye.Example.Spa.%1.csproj -c Release -f netcoreapp3.0 -r %2 -o Publish/%1 > NUL
EXIT /B
