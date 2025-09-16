var subscriptionId = externalInput('sys.envVar', 'AZURE_SUBSCRIPTION_ID')
var resourceGroup = externalInput('sys.envVar', 'AZURE_RESOURCE_GROUP')

using 'main.bicep' with {
  mode: 'deployment'
  scope: '/subscriptions/${subscriptionId}/resourceGroups/${resourceGroup}'
}

param containerName = 'foo'

