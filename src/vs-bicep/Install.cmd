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

choice /M "Uninstall first? (Y/N)"
if errorlevel 2 goto skip_uninstall
taskkill /im devenv.exe /t /f 2>&1 | findstr /v "not found"
call "%VSIXInstallerExe%" /uninstall:ms-azuretools.visualstudio-bicep
goto do_install

:skip_uninstall

choice /M "Install? (Y/N)"
if errorlevel 2 goto skip_install

:do_install
taskkill /im devenv.exe /t /f 2>&1 | findstr /v "not found"
call "%VSIXInstallerExe%" "%BicepVsixPath%"

:skip_install
