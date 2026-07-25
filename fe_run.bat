@echo off
setlocal

set "WEBCLIENT_DIR=%~dp0VivuCarClient\WebClient"

if not exist "%WEBCLIENT_DIR%\WebClient.csproj" (
    echo [ERROR] WebClient project not found: "%WEBCLIENT_DIR%"
    exit /b 1
)

pushd "%WEBCLIENT_DIR%"

echo [1/4] Installing frontend dependencies...
call npm ci
if errorlevel 1 goto :fail

echo [2/4] Building Tailwind CSS...
call npm run build:css
if errorlevel 1 goto :fail

echo [3/4] Building VivuCar WebClient...
dotnet build --nologo -p:UseAppHost=false
if errorlevel 1 goto :fail

echo [4/4] Starting VivuCar WebClient at http://localhost:5162...
set "ASPNETCORE_ENVIRONMENT=Development"
set "ASPNETCORE_URLS=http://localhost:5162"
start "" /b /wait dotnet ".\bin\Debug\net8.0\WebClient.dll"
popd
exit /b

:fail
set "EXIT_CODE=%ERRORLEVEL%"
echo [ERROR] Frontend startup stopped with exit code %EXIT_CODE%.
popd
exit /b %EXIT_CODE%
