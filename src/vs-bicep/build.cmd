@if (%_echo%)==() echo off

setlocal enabledelayedexpansion

set CONFIGURATION=%1
if (%CONFIGURATION%)==() set CONFIGURATION=debug
echo Selected configuration: %CONFIGURATION%

set VSWHERE=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe

for /f "usebackq tokens=*" %%i in (`"%VSWHERE%" -prerelease -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) do (
  echo "%%i" %*
  set MSBUILD=%%i
)

if "%MSBUILD%"=="" (
    echo Couldn't find msbuild
    exit /b 1
)

echo Found msbuild at %MSBUILD%

"%MSBUILD%" BicepInVisualStudio.sln /restore /p:Configuration=%CONFIGURATION% /v:m -p:RestorePackagesPath=packages /bl:bicep_in_visual_studio_build.binlog
