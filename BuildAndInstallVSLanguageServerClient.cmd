@echo off
setlocal

set AllParams=%*
set BuildConfiguration=Debug
call %~dp0\..\..\tools\EnsureExtensionsEnv.cmd

:ParseParams
set PARAM=%1
if "%PARAM%"=="" goto :DoneParseParams
if /i "%PARAM%"=="release" set BuildConfiguration=Release
shift & goto :ParseParams
:DoneParseParams

:: Builds faster if you use more processes than physical processors.  In my tests on a 12-core processor,
:: 4x gave the best performance.
set /a NumProcesses=%NUMBER_OF_PROCESSORS% * 4

set BuildTimeStart=%TIME%

msbuild.exe /m:%NumProcesses% /v:m /r /t:Build "bicep.sln" /p:Configuration=%BuildConfiguration%
if errorlevel 1 exit /b %errorlevel%

set BuildTimeEnd=%TIME%

set InstallTimeStart=%TIME%

call "%ExtensionsRoot%\src\Languages\InstallUpdates.cmd" %AllParams%
if errorlevel 1 exit /b %errorlevel%

set InstallTimeEnd=%TIME%

echo.
echo Build Duration:   %BuildTimeStart% - %BuildTimeEnd%
echo Install Duration: %InstallTimeStart% - %InstallTimeEnd%
