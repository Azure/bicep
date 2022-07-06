@echo off

SET VsWhereExePath=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe
set "ExtensionsRoot=%~dp0"

for /f "usebackq delims=" %%i in (`"%VsWhereExePath%" -latest -prerelease -products * -property enginePath`) do (
  set VSIXInstallerExePath=%%i
)

echo VSIXIntallerPath %VSIXInstallerExePath%

if exist "%VSIXInstallerExePath%\VSIXInstaller.exe" (
  SET BicepVsixPath=%~dp0Bicep.VSLanguageServerClient.Vsix\bin\Release\Bicep.VSLanguageServerClient.Vsix.vsix

  if exist %BicepVsixPath% (
    echo Installing: %BicepVsixPath%
      call "%VSIXInstallerExePath%\VSIXInstaller.exe" /quiet %BicepVsixPath%
    ) else (
      echo Failed to install vsix: %BicepVsixPath%
    )
)