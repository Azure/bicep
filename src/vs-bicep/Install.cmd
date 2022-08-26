@echo off

taskkill /im devenv.exe /t /f

set VsWhereExePath=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe
set "ExtensionsRoot=%~dp0"

for /f "usebackq delims=" %%i in (`"%VsWhereExePath%" -latest -prerelease -products * -property enginePath`) do (
  set VSIXInstallerExePath=%%i
)

set VSIXInstallerExe=%VSIXInstallerExePath%\VSIXInstaller.exe
echo VSIXInstaller.exe location: "%VSIXInstallerExe%"

set BicepVsixPath=%ExtensionsRoot%Bicep.VSLanguageServerClient.Vsix\bin\Release\vs-bicep.vsix
echo Bicep vsix location: %BicepVsixPath%

call "%VSIXInstallerExe%" /quiet "%BicepVsixPath%"