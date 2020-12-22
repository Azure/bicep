# Resource Scopes
## Introduction / Motivation
A deployment in ARM has an associated scope, which dictates the scope that resources within that deployment are created in. There are various ways to deploy resources across multiple scopes today in ARM templates; this spec describes how similar functionality can be achieved in Bicep.

See [here][arm-scopes] for more information on ARM scopes.

## Declaring and using scopes

### Declaring the target scope
Unless otherwise specified, Bicep will assume that a given `.bicep` file is to be deployed at a resource group scope, and will validate resources accordingly. If you wish to change this scope, or define a file that can be deployed at multiple scopes, you must use the `targetScope` keyword with either a string or array value as follows:

```bicep
// this file can only be deployed at a subscription scope
targetScope = 'subscription'
```

> **NOTE:** The below syntax to target multiple scopes below has not yet been implemented.
```bicep
// this file can be deployed at either a tenant or managementGroup scope
targetScope = [
  'tenant'
  'managementGroup'
]
```

The following strings are permitted for the `targetScope` keyword: `'tenant'`, `'managementGroup'`, `'subscription'`, `'resourceGroup'`. Expressions are not permitted.

It is important to set the target scope because it allows Bicep to perform validation that the resources declared in the `.bicep` file are permitted at that scope, and it also ensures that the correct type of scope is passed to the module when the module is referenced.

### Module 'scope' property
When declaring a module, you can supply a property named `scope` to set the scope at which to deploy the module. If the module's target scope is the same as the parent's target scope, this property may be omitted.

Assigning a scope to this field indicates that the module must be deployed at that scope. If the field is not provided, the module will be deployed at the target scope for the file (see [Declaring the target scope(s)](#declaring-the-target-scopes)).

```bicep
module myModule './path/to/module.bicep' = {
  name: 'myModule'
  // deploy this module at the subscription scope
  scope: scope.subscription()
}

var otherRgScope = scope.resourceGroup(otherSubscription, otherResourceGroupName)
module myModule './path/to/module.bicep' = {
  name: 'myModule'
  // deploy this module into a different resource group
  scope: otherRgScope
}
```

### Global Functions
The following functions will return a scope object which can be passed to an above-mentioned `scope` property:

```bicep
tenant() // returns the tenant scope

managementGroup() // returns the current management group scope (only from managementGroup deployments)
managementGroup(name: string) // returns the scope for a named management group

subscription() // returns the subscription scope for the current deployment (only from subscription & resourceGroup deployments)
subscription(subscriptionId: string) // returns a named subscription scope (only from subscription & resourceGroup deployments)

resourceGroup() // returns the current resource group scope (only from resourceGroup deployments)
resourceGroup(resourceGroupName: string) // returns a named resource group scope (only from subscription & resourceGroup deployments)
resourceGroup(subscriptionId: string, resourceGroupName: string) // returns a named resource group scope (only from subscription & resourceGroup deployments)
```

### Resource 'scope' Property
It is possible to define extension resources by supplying a reference to the resource being extended to the `scope` property of another resource. Unlike module scopes, Bicep currently only supports `resource` scopes being passed to resources. Using the parent resource scope will set up an implicit dependency from extension resource on parent resource.

```bicep
// deploy a parent storage account resource
resource storageAcc 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: accountName
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  location: resourceGroup().location
}

// declare a lock resource extending the storage account by supplying the 'scope' property
resource lockResource 'Microsoft.Authorization/locks@2016-09-01' = {
  name: 'DontDelete'
  scope: storageAcc
  properties: {
    level: 'CanNotDelete'
  }
}
```

## Example Usages
```bicep
// set the target scope for this file
targetScope = 'subscription'

// deploy a resource group to the subscription scope
resource myRg 'Microsoft.Resources/resourceGroups@2020-01-01' = {
  name: 'myRg'
  location: 'West US'
  scope: subscription()
}
var rgScope = resourceGroup('myRg') // use the scope of the newly-created resource group

// deploy a module to that newly-created resource group
module myMod './path/to/module.bicep' = {
  name: 'myMod'
  scope: rgScope
}
```

## Allowed combinations of scopes
This feature is limited to the same scoping constraints that exist within ARM Deployments today.

## Possible Extensions

### Parent-child syntax
We may want to use a similar concept to deploy child resources of a parent in a less-verbose manner - e.g.:
```bicep
resource myParent 'My.Rp/parentType@2020-01-01' = {
  name: 'myParent'
  location: 'West US'
}

resource myChild 'My.Rp/parentType/childType@2020-01-01' = {
  scope: myParent // pass parent reference
  name: 'myChild' // don't require the full name to be formatted with '/' characters
}
```

[arm-scopes]: https://docs.microsoft.com/en-us/azure/azure-resource-manager/management/overview#understand-scope
