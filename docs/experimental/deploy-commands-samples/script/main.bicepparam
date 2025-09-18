var subscriptionId = readCliArg('subscription-id')
var resourceGroup = readCliArg('resource-group')

using 'main.bicep' with {
  mode: 'deployment'
  // TODO improve on this
  scope: '/subscriptions/${subscriptionId}/resourceGroups/${resourceGroup}'
}

param containerName = 'foo'
