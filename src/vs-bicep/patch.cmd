@if "%_echo%"=="" echo off
setlocal enableextensions

set DEST="%~1"

if %DEST% equ "" (
    goto help
)

if not exist %DEST%\Bicep.VSLanguageServerClient.dll goto badpath
if not exist %DEST%\LanguageServer\Bicep.LangServer.exe goto badpath
goto :start

:badpath
echo ** The specified location does not appear to be a valid Bicep for Visual Studio extension directory. **
echo.
goto help

:help
echo You can quickly patch an existing installation if you know the correct destination location
echo   (you can figure this out with the VS "modules" window after attaching to devenv.exe)
echo.
echo Example (note the quotes):
echo.
echo   patch "C:\Users\<user>\AppData\Local\Microsoft\VisualStudio\17.0_a219d5e7Exp\Extensions\Microsoft\Bicep for Visual Studio\0.28.171.25288"
exit /b 1

:start

xcopy /s Bicep.VSLanguageServerClient\bin\Debug\net472\*.* %DEST%
xcopy /s ..\Bicep.LangServer\bin\Debug\net10.0\*.* %DEST%\LanguageServer
