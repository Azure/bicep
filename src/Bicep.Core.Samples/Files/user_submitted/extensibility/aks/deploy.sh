#!/bin/bash
# Deploys main.bicep in this folder

# Enable Azure CLI & VSCode for extensibility
export BICEP_IMPORTS_ENABLED_EXPERIMENTAL=true

scriptPath=$(dirname "$0")
baseName="bicepkubedemo"
adminUsername="anthony"

az deployment sub create \
  -f "$scriptPath/main.bicep" \
  --location 'West Central US' \
  --name $baseName \
  --parameters \
    baseName=$baseName \
    dnsPrefix=$baseName \
    linuxAdminUsername=$adminUsername \
    sshRSAPublicKey="$(cat ~/.ssh/id_rsa.pub)" \
  --query properties.outputs.webUrl