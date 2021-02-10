@description('Region where you resources are located')
param location string = resourceGroup().location
@description('Name of the Network Watcher attached to your subscription. Format: NetworkWatcher_<region_name>')
param NetworkWatcherName string = 'NetworkWatcher_${location}'
@description('Chosen name of your Flow log resource')
param FlowLogName string = 'FlowLog1'
@description('Resource ID of the target NSG')
param existingNSG string
@minValue(0)
@maxValue(365)
@description('Retention period in days. Default is zero which stands for permanent retention. Can be any Integer from 0 to 365')
param RetentionDays int = 0
@allowed([
  '1'
  '2'
])
@description('FlowLogs Version. Correct values are 1 or 2 (default)')
param FlowLogsversion string = '2'
@allowed([
  'Standard_LRS'
  'Standard_GRS'
  'Standard_ZRS'
])
@description('Storage Account type')
param storageAccountType string = 'Standard_LRS'

var storageAccountName_var = 'flowlogs${uniqueString(resourceGroup().id)}'

resource storageAccountName 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: storageAccountName_var
  location: location
  sku: {
    name: storageAccountType
  }
  kind: 'StorageV2'
  properties: {}
}

module deployFlowLogs './nested_deployFlowLogs.bicep' = {
  name: 'deployFlowLogs'
  scope: resourceGroup('NetworkWatcherRG')
  params: {
    location: location
    NetworkWatcherName: NetworkWatcherName
    FlowLogName: FlowLogName
    existingNSG: existingNSG
    RetentionDays: RetentionDays
    FlowLogsversion: FlowLogsversion
    storageAccountResourceId: storageAccountName.id
  }
}
