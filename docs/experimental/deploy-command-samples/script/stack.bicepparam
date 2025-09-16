var subscriptionId = externalInput('sys.envVar', 'AZURE_SUBSCRIPTION_ID')
var resourceGroup = externalInput('sys.envVar', 'AZURE_RESOURCE_GROUP')

using 'main.bicep' with {
  mode: 'stack'
  scope: '/subscriptions/${subscriptionId}/resourceGroups/${resourceGroup}'
  actionOnUnmanage: {
    resources: 'delete'
  }
  denySettings: {
    mode: 'denyDelete'
  }
}

param containerName = 'foo'
