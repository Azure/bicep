module storageAccountModuleV1 'ts:e21305d9-eef2-4990-8ed2-e2748236bee3/bicep-ci/storageAccountSpec-ff:v1' = {
  name: 'storageAccountModuleV1'
  params: {
    sku: 'Standard_LRS'
  }
}

// TODO: use aliases.
module storageAccountModuleV2 'ts:E21305D9-EEF2-4990-8ED2-E2748236BEE3/BICEP-CI/STORAGEACCOUNTSPEC-FF:V2' = {
  name: 'storageAccountModuleV2'
  params: {
    sku: 'Standard_GRS'
    location: 'westus'
  }
}

module webAppModuleV1 'ts:e21305d9-eef2-4990-8ed2-e2748236bee3/bicep-ci/webAppSpec-ff:1.0.0' = {
  name: 'webAppModuleV1'
}
