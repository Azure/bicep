param vmName string
param volumeType string = 'All'
param forceUpdateTag string = uniqueString(resourceGroup().id, deployment().name)
param location string = resourceGroup().location

resource vmExt 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
  name: '${vmName}/AzureDiskEncryption'
  location: location
  properties: {
    publisher: 'Microsoft.Azure.Security'
    type: 'AzureDiskEncryption'
    typeHandlerVersion: '2.2'
    autoUpgradeMinorVersion: true
    forceUpdateTag: forceUpdateTag
    settings: {
      EncryptionOperation: 'DisableEncryption'
      VolumeType: volumeType
    }
  }
}
