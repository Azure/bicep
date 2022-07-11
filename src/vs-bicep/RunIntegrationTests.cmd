@echo off

set VsWhereExePath=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe
set "ExtensionsRoot=%~dp0"

set vswhere=

for /d %%i in ("%ProgramFiles(x86)%","%ProgramFiles%") do (
	if exist "%%~i\Microsoft Visual Studio\Installer\vswhere.exe" (
		set "vswhere=%%~i\Microsoft Visual Studio\Installer\vswhere.exe"
		break
	)
)

if [vswhere] equ [] (
	echo Error: please make sure you have installed Visual Studio 2017 or later.
	exit /b 1
)

for /f "usebackq delims=" %%i in (`"%VsWhereExePath%" -latest -products * -requires Microsoft.VisualStudio.Workload.ManagedDesktop Microsoft.VisualStudio.Workload.Web -requiresAny -property installationPath`) do (
  set InstallDir=%%i
)

echo VS install directory: %InstallDir%

set VsTestConsoleExePath="%InstallDir%\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"
echo vstest.console.exe location: "%VsTestConsoleExePath%"

set IntegrationTestsDllPath=%ExtensionsRoot%Bicep.VSLanguageServerClient.IntegrationTests\bin\Release\net472\Bicep.VSLanguageServerClient.IntegrationTests.dll
echo Integration tests dll location: %IntegrationTestsDllPath%

call "%InstallDir%\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" "%IntegrationTestsDllPath%"
