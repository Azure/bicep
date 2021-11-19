# VSCode extension itself does not change that much and majority of changes are done in Language Server or Bicep.Core
# This script allows you to quickly launch your existing VSCode with Bicep Extension installed but with your debug build of Bicep.LangServer and Bicep.CLI debug output added o the Path
# any agruments passed to this script is given to the VSCode executable, therefore you can run
# ./bcode.ps1 docs/Examples to open VSCode in the Examples folder running your local bicep lang server and in vscode terminal you have access to your local bicep CLI build

$ENV:BICEP_LANGUAGE_SERVER_PATH=[System.IO.Path]::Join($PSScriptRoot,"..\src\Bicep.LangServer\bin\Debug\net6.0\Bicep.LangServer.dll")
$ENV:Path=[System.IO.Path]::Join($PSScriptRoot,"..\src\Bicep.Cli\bin\Debug\net6.0") + ";" + $ENV:Path
& Start-Process -FilePath "code" -WindowStyle hidden -ArgumentList $args
