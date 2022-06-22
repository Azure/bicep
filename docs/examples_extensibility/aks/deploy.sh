#!/bin/bash

# In order to use this functionality, you must first have been enrolled in the extensibility preview.
# You will also need to ensure you've set the BICEP_IMPORTS_ENABLED_EXPERIMENTAL env var to 'true' in both VSCode and your CLI environment.

baseName="bicepkubedemo"
adminUsername="anthony"

# end-to-end deployment
az deployment sub create \
  -f ./docs/examples/extensibility/aks/main.bicep \
  --location 'West Central US' \
  --name $baseName \
  --parameters \
    baseName=$baseName \
    dnsPrefix=$baseName \
    linuxAdminUsername=$adminUsername \
    sshRSAPublicKey="$(cat ~/.ssh/id_rsa.pub)" \
  --query properties.outputs.webUrl

# deploy aks individually
az deployment group create \
  -f ./docs/examples/extensibility/aks/modules/aks.bicep \
  --resource-group $baseName \
  --parameters \
    baseName=$baseName \
    dnsPrefix=$baseName \
    linuxAdminUsername=$adminUsername \
    sshRSAPublicKey="$(cat ~/.ssh/id_rsa.pub)" \
  --query properties.outputs.kubeconfig

# deploy kubernetes individually
kubeConfig=<base64 encoded kubeconfig>
az deployment group create \
  -f ./docs/examples/extensibility/aks/modules/kubernetes.bicep \
  --resource-group $baseName \
  --parameters \
    kubeConfig=$kubeConfig