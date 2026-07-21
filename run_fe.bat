@echo off
setlocal

set "WEBCLIENT_DIR=%~dp0VivuCarClient\WebClient"

if not exist "%WEBCLIENT_DIR%\WebClient.csproj" (
    echo [ERROR] WebClient project not found: "%WEBCLIENT_DIR%"
    exit /b 1
)

pushd "%WEBCLIENT_DIR%"

echo [1/3] Installing frontend dependencies...
call npm ci
if errorlevel 1 goto :fail

echo [2/3] Building Tailwind CSS...
call npm run build:css
if errorlevel 1 goto :fail

echo [3/3] Starting VivuCar WebClient...
dotnet run
set "EXIT_CODE=%ERRORLEVEL%"
popd
exit /b %EXIT_CODE%

:fail
set "EXIT_CODE=%ERRORLEVEL%"
echo [ERROR] Frontend startup stopped with exit code %EXIT_CODE%.
popd
exit /b %EXIT_CODE%
