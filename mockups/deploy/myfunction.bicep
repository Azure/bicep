param name string
param storageName string
param location string
param hostingPlanName string

@secure()
param msdeployPackageUrl string

resource name_resource 'Microsoft.Web/sites@2020-06-01' = {
  kind: 'functionapp'
  name: name
  location: location
  properties: {
    name: name
    serverFarmId: hostingPlan.id
    siteConfig: {
      appSettings: [
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageName};AccountKey=${listKeys(resourceId('Microsoft.Storage/storageAccounts',storageName),'2015-05-01-preview').key1}'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~2'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageName};AccountKey=${listKeys(resourceId('Microsoft.Storage/storageAccounts',storageName),'2015-05-01-preview').key1}'
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: toLower(name)
        }
        {
          name: 'WEBSITE_NODE_DEFAULT_VERSION'
          value: '10.14.1'
        }
      ]
    }
  }
}

resource name_MSDeploy 'Microsoft.Web/sites/Extensions@2019-08-01' = {
  parent: name_resource
  name: 'MSDeploy'
  properties: {
    packageUri: msdeployPackageUrl
  }
}

resource hostingPlan 'Microsoft.Web/serverfarms@2015-04-01' = {
  name: hostingPlanName
  location: location
  properties: {
    name: hostingPlanName
    computeMode: 'Dynamic'
    sku: 'Dynamic'
  }
}
