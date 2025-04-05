param webApp {
  siteName: string
  @secure()
  servicePackageLink: string
  hostingPlanName: string
  sku: string
  workerSize: string
}

param storage {
  location: string
  storageAccountName: string
  accountType: string
  kind: string
  accessTier: string
  supportsHttpsTrafficOnly: bool
}

param functionApp {
  name: string
  storageName: string
  location: string
  hostingPlanName: string
  @secure()
  msdeployPackageUrl: string
}

param keyVault {
  keyVaultName: string
  tenantId: string
  location: string
  secretName: string
  objectId: string
  functionName: string
}

module deployWebApp 'mywebapp.bicep' = {
  name: 'webapp'
  params: {
    siteName: webApp.siteName
    servicePackageLink: webApp.servicePackageLink
    hostingPlanName: webApp.hostingPlanName
    workerSize: webApp.workerSize
    sku: webApp.sku
  }
}

module deployStorage 'mystorage.bicep' = {
  name: 'storage'
  params: {
    location: storage.location
    kind: storage.kind
    accessTier: storage.accessTier
    accountType: storage.accountType
    storageAccountName: storage.storageAccountName
    supportsHttpsTrafficOnly: storage.supportsHttpsTrafficOnly
  }
}


module deployFunctionApp 'myfunction.bicep' = {
  name: 'functionApp'
  params: {
    name: functionApp.name
    storageName: functionApp.storageName
    location: functionApp.location
    hostingPlanName: functionApp.hostingPlanName
    msdeployPackageUrl: functionApp.msdeployPackageUrl
  }
}


module deployKeyVault 'mykeyvault.bicep' = {
  name: 'keyvault'
  params: {
    functionName: keyVault.functionName
    keyVaultName: keyVault.keyVaultName
    location: keyVault.location
    objectId: keyVault.objectId
    secretName: keyVault.secretName
    tenantId: keyVault.tenantId
  }
}
