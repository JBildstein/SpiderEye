@ECHO OFF
SETLOCAL

CD ..\SpiderEye.Playground.Core
ECHO Installing node packages...
CALL npm i > NUL 2>&1
IF %ERRORLEVEL% NEQ 0 (
  ECHO Installing node packages failed
  EXIT /B 1
)

ECHO Linting files...
CALL npm run lint > NUL
IF %ERRORLEVEL% NEQ 0 (
  ECHO Linting failed
  EXIT /B 1
)

ECHO Building Angular...
CALL npm run build:prod > NUL
IF %ERRORLEVEL% NEQ 0 (
  ECHO Angular build failed
  EXIT /B 1
)

CD ..
CALL :PublishProject Windows win-x64 net5.0-windows
IF %ERRORLEVEL% NEQ 0 (
  ECHO Publish failed
  EXIT /B 1
)

CALL :PublishProject Linux linux-x64 net5.0
IF %ERRORLEVEL% NEQ 0 (
  ECHO Publish failed
  EXIT /B 1
)

CALL :PublishProject Mac osx-x64 net5.0
IF %ERRORLEVEL% NEQ 0 (
  ECHO Publish failed
  EXIT /B 1
)

ECHO Done!

EXIT /B 0

:PublishProject
ECHO Publishing for %1...
CALL dotnet publish SpiderEye.Playground.%1 -c Release -f %3 -r %2 -o Publish/%1 > NUL
EXIT /B
