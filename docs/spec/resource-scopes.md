# Resource Scopes

## Introduction / Motivation

A deployment in ARM has an associated scope, which dictates the scope that resources within that deployment are created in. There are various ways to deploy resources across multiple scopes today in ARM templates; this spec describes how similar functionality can be achieved in Bicep.

[Read more about ARM scopes][arm-scopes].

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

When declaring a module, you can supply an optional property named `scope` to set the scope at which to deploy the module. By default, bicep assumes the module will target the same scope as the parent if this property is omitted.

Assigning a scope to this field indicates that the module must be deployed at that scope. If the field is not provided, the module will be deployed at the target scope for the file (see [Declaring the target scope(s)](#declaring-the-target-scopes)).

```bicep
module myModule './path/to/module.bicep' = {
  name: 'myModule'
  // deploy this module at the subscription scope
  scope: subscription()
}

module myModule './path/to/module.bicep' = {
  name: 'myModule'
  // deploy this module into a different resource group
  scope: resourceGroup(otherSubscription, otherResourceGroupName)
}
```

### Global Functions

The following functions will return a scope object, which can be passed to an above-mentioned `scope` property:

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

### Parent-child syntax

There are two ways to declare child resources in Bicep. One as a resource nested inside of the parent, the other as a top-level resource with a reference to the parent symbolic name.

#### Nested child syntax

The `resource` keyword can be declared inside of the parent in the same way as a top-level resource: 

```bicep
resource myParent 'My.Rp/parentType@2020-01-01' = {
  name: 'myParent'
  location: 'West US'

  resource myChild 'childType' = { // only need to specify child type
    name: 'myChild'
  }
} 
```

With this syntax, you do not need to specify the full resource type, only the child type. You also do not need to specify the api version, as Bicep will assume it is the same as the parent. If needed, the api version of the child can be optionally overridden to something different than the parent.

When using the nested child syntax, you need to use the `::` operator to access the child symbol. For example, if you need to output a property from the child you write the following:

```bicep
output childProp string = myParent::myChild.properties.someProp
```

More info on the nested child resource access operator can be found in the [expressions spec](./expressions.md#nested-resource-accessors).

#### "parent" property syntax

Alternatively, you can declare the child resource as a top-level resource just like the parent. To do this, specify the `parent` property on the child with the value set to the symbolic name of the parent. With this syntax you still need to declare the full resource type, but the `name` of the child resource is only the name of the child.

```bicep
resource myParent 'My.Rp/parentType@2020-01-01' = {
  name: 'myParent'
  location: 'West US'
}

resource myChild 'My.Rp/parentType/childType@2020-01-01' = {
  parent: myParent // pass parent reference
  name: 'myChild' // don't require the full name to be formatted with '/' characters
}

output childProp string = myChild.properties.someProp
```

In this case, referencing the child resource works the same as referencing the parent.

**Note:** the `name` property rules are different than ARM Templates, which requires concatenating the parent and child name together.

## Allowed combinations of scopes

This feature is limited to the same scoping constraints that exist within ARM Deployments today.

## Example Usages

If you have a symbolic reference to a scope, you can use that as a value of the `scope` property. Currently this only works with the `resourceGroup` scope, it does not yet support `subscription` or `managementGroup` scope types.

```bicep
// set the target scope for this file
targetScope = 'subscription'

// deploy a resource group to the subscription scope
resource myRg 'Microsoft.Resources/resourceGroups@2020-01-01' = {
  name: 'myRg'
  location: 'West US'
  scope: subscription()
}

// deploy a module to that newly-created resource group
module myMod './path/to/module.bicep' = {
  name: 'myMod'
  scope: myRg
}
```

[arm-scopes]: https://docs.microsoft.com/azure/azure-resource-manager/management/overview#understand-scope