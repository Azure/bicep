var subscriptionId = readCliArg('subscription-id')
//@[4:18) Variable subscriptionId. Type: string. Declaration start char: 0, length: 50
var resourceGroup = readCliArg('resource-group')
//@[4:17) Variable resourceGroup. Type: string. Declaration start char: 0, length: 48

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

