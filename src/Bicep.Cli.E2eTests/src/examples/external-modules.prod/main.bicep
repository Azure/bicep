module storageAccountModuleV1 'ts:61e0a28a-63ed-4afc-9827-2ed09b7b30f3/bicep-ci/storageAccountSpec-df:v1' = {
  name: 'storageAccountModuleV1'
  params: {
    sku: 'Standard_LRS'
  }
}

// TODO: use aliases.
module storageAccountModuleV2 'ts:61E0A28A-63ED-4AFC-9827-2ED09B7B30F3/BICEP-CI/STORAGEACCOUNTSPEC-DF:V2' = {
  name: 'storageAccountModuleV2'
  params: {
    sku: 'Standard_GRS'
    location: 'westus'
  }
}

module webAppModuleV1 'ts:61e0a28a-63ed-4afc-9827-2ed09b7b30f3/bicep-ci/webAppSpec-df:1.0.0' = {
  name: 'webAppModuleV1'
}
