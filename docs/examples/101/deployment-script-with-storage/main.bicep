param location string = resourceGroup().location
param scriptToExecute string = 'date' // will print current date & time on container
param subId string = subscription().id // defaults to current sub
param rgName string = resourceGroup().name // defaults to current rg
param uamiName string = 'alex-test-deny'

param currentTime string = utcNow()

var uamiId = resourceId(subId, rgName, 'Microsoft.ManagedIdentity/userAssignedIdentities', uamiName)

resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'dscript${uniqueString(resourceGroup().id)}'
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

resource dScript 'Microsoft.Resources/deploymentScripts@2019-10-01-preview' = {
  name: 'scriptWithStorage'
  location: location
  kind: 'AzureCLI'
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
<<<<<<< HEAD:docs/examples/101/deployment-script-with-storage/main.bicep
      // todo - add expression once properties can be set as strings
      '${uamiId}': {}
=======
      '${uamiId}': { }
>>>>>>> fc828515a8c5963f9e186aeac60afd7e2497f907:docs/examples/not-working-yet/deployment-script-with-storage/main.bicep
    }
  }
  properties: {
    azCliVersion: '2.0.80'
    storageAccountSettings: {
      storageAccountName: stg.name
      storageAccountKey: listKeys(stg.id, stg.apiVersion).keys[0].value
    }
    scriptContent: scriptToExecute
    cleanupPreference: 'OnSuccess'
    retentionInterval: 'P1D'
    forceUpdateTag: currentTime // ensures script will run every time
  }
}

// print logs from script after template is finished deploying
output scriptLogs string = reference('${dScript.id}/logs/default', dScript.apiVersion, 'Full').properties.log