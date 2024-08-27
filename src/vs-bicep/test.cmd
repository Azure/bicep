@if (%_echo%)==() echo off

setlocal enabledelayedexpansion

set CONFIGURATION=%1
if (%CONFIGURATION%)==() set CONFIGURATION=debug
echo Selected configuration: %CONFIGURATION%

set VSWHERE=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe

for /f "usebackq tokens=*" %%i in (`"%VSWHERE%" -prerelease -latest -requiresAny -find **\TestPlatform\vstest.console.exe`) do (
  echo "%%i" %*
  set VSTESTCONSOLE=%%i
)

if "%VSTESTCONSOLE%"=="" (
    echo Couldn't find vstest.console.exe
    exit /b 1
)

echo Found vstest.console.exe at %VSTESTCONSOLE%

"%VSTESTCONSOLE%" Bicep.VSLanguageServerClient.UnitTests\bin\Debug\net472\*UnitTests.dll

