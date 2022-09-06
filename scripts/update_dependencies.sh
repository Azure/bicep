#!/bin/bash

# This scripts assumes that the following commands have already been run to install dependencies:
# > npm install -g npm-check-updates
# > dotnet tool install -g dotnet-outdated-tool

NPM_PATHS=(
  "src/vscode-bicep"
  "src/playground"
  "src/Bicep.Cli.E2eTests"
  "src/Bicep.MSBuild.E2eTests"
  "src/textmate"
  "src/highlightjs"
  "src/monarch"
)

for NPM_PATH in "${NPM_PATHS[@]}"; do
  pushd $NPM_PATH
  npm-check-updates -u
  npm install
  # undo the changes if npm install fails
  if [ $? -ne 0 ] 
  then
    git checkout -- "package.json"
    git checkout -- "package-lock.json"
  fi
  popd
done

unset DOTNET_ROOT
dotnet restore
dotnet outdated -u --no-restore
dotnet restore --force-evaluate