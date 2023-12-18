#!/bin/bash
set -e
basePath=/Users/ant/Code/bicep-local
outPath="$basePath/src/Bicep.LocalDeploy/demo/build"

dotnet publish --configuration release --self-contained true -p:PublishSingleFile=true -r osx-arm64 "$basePath/src/Bicep.Cli/Bicep.Cli.csproj"
dotnet publish --configuration release --self-contained true -p:PublishSingleFile=true -r win-x64 "$basePath/src/Bicep.Cli/Bicep.Cli.csproj"

npm --prefix "$basePath/src/vscode-bicep" ci
npm --prefix "$basePath/src/vscode-bicep" run package

rm -Rf "$outPath" 
mkdir "$outPath"

cp -R "$basePath/src/Bicep.LocalDeploy/Samples" "$outPath/samples"
cp "$basePath/src/vscode-bicep/vscode-bicep.vsix" "$outPath/vscode-bicep.vsix"
cp "$basePath/src/Bicep.Cli/bin/release/net7.0/osx-arm64/publish/bicep" "$outPath/bicep-osx-arm64"
cp "$basePath/src/Bicep.Cli/bin/release/net7.0/win-x64/publish/bicep.exe" "$outPath/bicep-win-x64.exe"
cp "$basePath/src/Bicep.LocalDeploy/demo/README.md" "$outPath/README.md"

pushd "$outPath"
zip -r "$outPath.zip" .
popd
rm -Rf "$outPath"