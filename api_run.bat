@echo off
cd /d "%~dp0VivuCarServer\API"
start "" /b /wait dotnet run
exit /b
