var subscriptionId = externalInput('sys.cliArg', 'subscription-id')
var resourceGroup = externalInput('sys.cliArg', 'resource-group')

using 'main.bicep' with {
  mode: 'deployment'
  // TODO improve on this
  scope: '/subscriptions/${subscriptionId}/resourceGroups/${resourceGroup}'
}

param containerName = 'foo'
