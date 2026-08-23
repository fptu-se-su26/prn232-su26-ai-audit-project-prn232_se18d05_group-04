@echo off
cd /d "%~dp0VivuCarServer\API"
echo Starting VivuCar API...
echo Press Ctrl+C to stop.
powershell -NoLogo -Command "dotnet run"
exit /b %ERRORLEVEL%
