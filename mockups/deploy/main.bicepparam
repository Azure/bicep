using './main.bicep'

var config = externalInput('ev2.getConfig', '$config')
var location = any(externalInput('ev2.getConfig', '$location'))

var sitePrefix = 'MySite'
var lowerCaseResourcePrefix = 'myservice'

param webApp = {
  sku: 'Free'
  hostingPlanName: '${lowerCaseResourcePrefix}WebApp-AppPlan'
  servicePackageLink: externalInput('ev2.sasUriForPath', {
    path: 'bin/WebApplication.zip'
  })
  siteName: '${sitePrefix}Ev2Sample${config.regionShortName}'
  workerSize: '1'
}

param storage = {
  location: location
  kind: 'StorageV2'
  accessTier: 'Hot'
  accountType: 'Standard_RAGRS'
  storageAccountName: '${lowerCaseResourcePrefix}storage${config.regionShortName}'
  supportsHttpsTrafficOnly: true
}

param functionApp = {
  name: '${config.lowerCaseResourcePrefix}functionapp${config.regionShortName}'
  location: location
  hostingPlanName: '${config.lowerCaseResourcePrefix}FunctionApp-AppPlan'
  msdeployPackageUrl: externalInput('ev2.sasUriForPath', {
    path: 'bin/Function.zip'
  })
  storageName: '${lowerCaseResourcePrefix}storage${config.regionShortName}'
}

param keyVault = {
  location: location
  functionName: '${lowerCaseResourcePrefix}functionapp${config.regionShortName}'
  keyVaultName: '${lowerCaseResourcePrefix}keyVault${config.regionShortName}'
  objectId: any(externalInput('ev2.getConfig', '$callerObjectId'))
  secretName: '${lowerCaseResourcePrefix}secret'
  tenantId: config.aad.tenants.Microsoft.id
}
