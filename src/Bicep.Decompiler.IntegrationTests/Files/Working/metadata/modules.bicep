param cdnProfileName string
param eventHubName string
param eventHubNamespace string
param eventHubLocation string

@description('Create EventHub.')
module cdnProfileName_eventhub_module './nested_cdnProfileName_eventhub_module.bicep' = {
  name: '${cdnProfileName}-eventhub-module'
  params: {
    eventHubNameSpace: eventHubNamespace
    eventHubName: eventHubName
    eventHubLocation: eventHubLocation
  }
}

