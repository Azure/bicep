targetScope = 'tenant'

@description('Management group id')
param managementGroupId string

@description('Management group display name')
param managementGroupDisplayName string

@description('Management group id of the parent management group')
param parentManagementGroupId string = ''

@description('Subscription id of the subscription(s) to add to the management group')
param subscriptionIds array

resource managementGroup 'Microsoft.Management/managementGroups@2020-05-01' = {
  name: managementGroupId
  properties: {
    displayName: managementGroupDisplayName
    details: {
      parent: {
        id: (!empty(parentManagementGroupId)) ? '/providers/Microsoft.Management/managementGroups/${parentManagementGroupId}' : null
      }
    }
  }
}

resource managementGroupsubscriptions 'Microsoft.Management/managementGroups/subscriptions@2020-05-01' = [for subscriptionId in subscriptionIds: {
  name: subscriptionId
  parent: managementGroup
}]

output managementGroupID string = managementGroup.name
