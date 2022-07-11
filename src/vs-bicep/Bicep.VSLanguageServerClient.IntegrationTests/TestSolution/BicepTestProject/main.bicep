@description('The globally unique name of the SignalR resource to create.')
param resourceId string = uniqueString(resourceGroup().id)
