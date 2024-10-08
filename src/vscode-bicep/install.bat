@echo off
for /f %%a in ('echo prompt $E^| cmd') do set "ESC=%%a"
set REDBG=%ESC%[41m
set NOCOLOR=%ESC%[%m

pushd ..\vscode-bicep-ui
@echo %REDBG%Installing vscode-bicep-ui dependencies%NOCOLOR%
call npm i

@echo %REDBG%Building vscode-bicep-ui%NOCOLOR%
call npm run build
popd

rem --install-links must be used when running npm i on this folder or the dependencies from vscode-bicep-ui will be wrong
@echo %REDBG%Installing vscode-bicep dependencies%NOCOLOR%
call npm i --install-links
