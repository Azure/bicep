:: Copyright (c) Microsoft. All rights reserved.
:: Licensed under the MIT license. See LICENSE file in the project root for full license information.

::Get the root path of Visual Studio installation folder and set it to vspath.

@echo off

set vswhere=

for /d %%i in ("%ProgramFiles(x86)%","%ProgramFiles%") do (
	if exist "%%~i\Microsoft Visual Studio\Installer\vswhere.exe" (
		set "vswhere=%%~i\Microsoft Visual Studio\Installer\vswhere.exe"
		break
	)
)

if [vswhere] equ [] (
	echo Error: please make sure you have installed Visual Studio 2022.
	exit /b 1
)

echo vswhere path: %vswhere%

set vspath=
set /A maxvsversion=0

for /f "usebackq tokens=1*" %%i in (`"%vswhere%"`) do (
    if %%i equ catalog_productLineVersion: (
        call :SetMaxVSVersion %%j
    )
)

goto EndSetMaxVSVersion

:SetMaxVSVersion
set /A previousmaxvsversion=%maxvsversion%
set /A maxvsversion=%1
if [%maxvsversion%] lss [%previousmaxvsversion%] (
    set /A maxvsversion=%previousmaxvsversion%
)
goto :eof

:EndSetMaxVSVersion

if [maxvsversion] equ [0] (
	echo Error: please make sure you have installed Visual Studio 2022.
	exit /b 1
)

for /f "usebackq tokens=1*" %%i in (`"%vswhere%"`) do (
    if %%i equ installationPath: (
        (echo "%%j" | findstr /i /c:"%maxvsversion%" >nul) && (set vspath=%%j)
    )
)

echo visual studio installation path: %vspath%

if [vspath] equ [] (
	echo Error: please make sure you have installed Visual Studio 2022.
	exit /b 1
)

echo visual studio installation path: %vspath%

exit /b 0