param location string {
  metadata: {
    description: 'Region where you resources are located'
  }
  default: resourceGroup().location
}
param NetworkWatcherName string {
  metadata: {
    description: 'Name of the Network Watcher attached to your subscription. Format: NetworkWatcher_<region_name>'
  }
  default: 'NetworkWatcher_${location}'
}
param FlowLogName string {
  metadata: {
    description: 'Chosen name of your Flow log resource'
  }
  default: 'FlowLog1'
}
param existingNSG string {
  metadata: {
    description: 'Resource ID of the target NSG'
  }
}
param RetentionDays int {
  minValue: 0
  maxValue: 365
  metadata: {
    description: 'Retention period in days. Default is zero which stands for permanent retention. Can be any Integer from 0 to 365'
  }
  default: 0
}
param FlowLogsversion string {
  allowed: [
    '1'
    '2'
  ]
  metadata: {
    description: 'FlowLogs Version. Correct values are 1 or 2 (default)'
  }
  default: '2'
}
param storageAccountType string {
  allowed: [
    'Standard_LRS'
    'Standard_GRS'
    'Standard_ZRS'
  ]
  metadata: {
    description: 'Storage Account type'
  }
  default: 'Standard_LRS'
}

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
