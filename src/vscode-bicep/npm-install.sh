#!/bin/bash

# Move to the vscode-bicep-ui directory
pushd ../vscode-bicep-ui

echo "Installing vscode-bicep-ui dependencies"
npm install

echo "Building vscode-bicep-ui"
npm run build

popd

# Install dependencies for the current folder with correct links
echo "Installing vscode-bicep dependencies"
npm i
