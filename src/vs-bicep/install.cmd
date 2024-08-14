@if (%_echo%)==() echo off

set CONFIGURATION=%1
if (%CONFIGURATION%)==() set CONFIGURATION=debug
echo Selected configuration: %CONFIGURATION%

set VsWhereExePath=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe
set "ExtensionsRoot=%~dp0"

for /f "usebackq delims=" %%i in (`"%VsWhereExePath%" -latest -prerelease -products * -property enginePath`) do (
  set VSIXInstallerExePath=%%i
)

set VSIXInstallerExe=%VSIXInstallerExePath%\VSIXInstaller.exe
echo VSIXInstaller.exe location: "%VSIXInstallerExe%"

set BicepVsixPath=%ExtensionsRoot%Bicep.VSLanguageServerClient.Vsix\bin\%CONFIGURATION%\vs-bicep.vsix
echo Bicep vsix location: %BicepVsixPath%

echo.
choice /M "Install? (Y/N)"
if errorlevel 2 goto end

echo.
echo Uninstalling current extension...
taskkill /im:Bicep.LangServer.exe /f
taskkill /im:devenv.exe /f
call "%VSIXInstallerExe%" /shutdownprocesses /quiet /uninstall:ms-azuretools.visualstudio-bicep

echo Installing extension...
call "%VSIXInstallerExe%" /shutdownprocesses /quiet /force "%BicepVsixPath%"

:end
