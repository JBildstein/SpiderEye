@ECHO OFF
SETLOCAL

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
CALL dotnet publish App.%1\SpiderEye.Example.Simple.%1.csproj -c Release -f netcoreapp3.0 -r %2 -o Publish/%1 > NUL
EXIT /B
