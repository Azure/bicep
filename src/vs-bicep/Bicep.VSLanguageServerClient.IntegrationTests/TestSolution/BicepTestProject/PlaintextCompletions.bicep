@description('The globally unique name of the SignalR resource to create.')
param name string = uniqueString(resourceGroup().id)
