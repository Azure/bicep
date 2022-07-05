#!/bin/bash
# Launches a new VSCode window with extensibility preview enabled

# Enable Azure CLI & VSCode for extensibility
export BICEP_IMPORTS_ENABLED_EXPERIMENTAL=true

scriptPath=$(dirname "$0")

code -n "$scriptPath/main.bicep"