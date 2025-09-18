var subscriptionId = readEnvVar('AZURE_SUBSCRIPTION_ID')
var resourceGroup = readEnvVar('AZURE_RESOURCE_GROUP')

using 'main.bicep' with {
  mode: 'deployment'
  scope: '/subscriptions/${subscriptionId}/resourceGroups/${resourceGroup}'
}

param containerName = 'foo'

