param storageName string = toLower('${take('deployscript${uniqueString(resourceGroup().id)}', 22)}st')
param containerName string = toLower('${take('deployscript${uniqueString(resourceGroup().id)}', 22)}ci')

@description('Specify which type of dev environment to deploy')
@allowed([
  'AzureCLI'
  'AzurePowerShell'
])
param type string = 'AzureCLI'

@description('Use to specify the version to use for Azure CLI or AzurePowerShell, if no version is specified latest will be used for AzCLI and 5.6 for AzPwsh')
param toolVersion string = ''

param location string = resourceGroup().location

module storage './storage.bicep' = {
  name: '${storageName}-deploy'
  params: {
    name: storageName
    location: location
  }
}

module container './containergroups.bicep' = {
  name: '${containerName}-deploy'
  params: {
    name: containerName
    location: location
    storageName: storage.outputs.storageName
    storageId: storage.outputs.resourceId
    fileShareName: storage.outputs.fileShareName
    type: type
    toolVersion: toolVersion
  }
}
