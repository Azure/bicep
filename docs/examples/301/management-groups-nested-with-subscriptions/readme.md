# Management Groups
This module will deploy a management group and add subscription(s) to it.

## Requirements
The principal conducting the deployment must have permissions to create resources at the tenant scope. 

More info can be found [here.](https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/deploy-to-tenant?tabs=azure-cli#required-access)

## Usage

### Example 1 - Management Group with tenant root as parent
``` bicep
targetScope = 'tenant'

param deploymentName string = 'managementGroups${utcNow()}'

module managementGroupGlobal './main.bicep' = {
  name: '${deploymentName}managementGroupGlobal'
  params: {
    managementGroupDisplayName: 'MyOrganisation'
    managementGroupId: 'gbl-org-mgp'
    subscriptionIds: []
  }
}
```

### Example 2 - Nested management group with subscriptions
``` bicep
targetScope = 'tenant'

param deploymentName string = 'managementGroups${utcNow()}'

module managementGroupGlobal './main.bicep' = {
  name: '${deploymentName}managementGroupGlobal'
  params: {
    managementGroupDisplayName: 'MyOrganisation'
    managementGroupId: 'gbl-org-mgp'
    subscriptionIds: []
  }
}

module managementGroupPlatform './main.bicep' = {
  name: '${deploymentName}managementGroupSandbox'
  params: {
    managementGroupDisplayName: 'Sandbox'
    managementGroupId: 'sbx-org-mgp'
    parentManagementGroupId: managementGroupGlobal.outputs.managementGroupID
    subscriptionIds: [
      '63c1651a-ec30-4f6c-a3ec-671e23063585'
    ]
  }
}
```