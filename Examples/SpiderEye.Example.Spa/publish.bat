@ECHO OFF

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

ECHO Publishing for Windows...
CALL dotnet publish -c Release -f net462 -r win -o ./bin/Publish/Windows > NUL
IF %ERRORLEVEL% NEQ 0 (
  ECHO Publish failed
  EXIT /B 1
)

ECHO Publishing for Linux...
CALL dotnet publish -c Release -f netcoreapp2.2 -r linux-x64 -o ./bin/Publish/Linux > NUL
IF %ERRORLEVEL% NEQ 0 (
  ECHO Publish failed
  EXIT /B 1
)

ECHO Publishing for Mac...
CALL dotnet publish -c Release -f netcoreapp2.2 -r osx-x64 -o ./bin/Publish/Mac > NUL
IF %ERRORLEVEL% NEQ 0 (
  ECHO Publish failed
  EXIT /B 1
)

ECHO Done!

EXIT /B 0
