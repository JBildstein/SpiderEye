@ECHO OFF

CALL dotnet pack -c Release --version-suffix %1 -o ./bin/Pack

EXIT /B 0
