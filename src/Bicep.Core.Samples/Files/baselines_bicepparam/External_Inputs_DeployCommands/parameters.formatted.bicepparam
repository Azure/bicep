var subscriptionId = readCliArg('subscription-id')
var resourceGroup = readCliArg('resource-group')

using 'main.bicep' with {
  scope: '/subscriptions/${subscriptionId}/resourceGroups/${resourceGroup}'
  actionOnUnmanage: {
    resources: 'delete'
  }
  denySettings: {
    mode: 'denyDelete'
  }
  mode: 'stack'
}
