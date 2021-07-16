# Resources

A `resource` declaration defines a resource that will be either created or updated at deployment time along with its intended state. The resource is also assigned to an identifier. You can reference the identifier in [expressions](./expressions.md) that are part of [variables](./variables.md), [outputs](./outputs.md), or other `resource` declarations.

Consider the following declaration that creates or updates a [DNS Zone](https://docs.microsoft.com/en-us/azure/dns/dns-zones-records):
```bicep
resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = {
  name: 'myZone'
  location: 'global'
}
```

## Declaration Components

A `resource` declaration consists of the following components:

- `resource` keyword
- An identifier (`dnsZone` in the example) that can be used in [expressions](./expressions.md) to reference the resource. The identifier does not impact the name of the resource. A resource cannot have the same identifier as any [parameter](./parameters.md), [variable](./variables.md), or another resource in the same scope.
- Resource type + API version (`Microsoft.Network/dnsZones` and `2018-05-01` in the example, respectively)
- Resource body. DNS Zones do not require complicated configuration, so we are only setting the `name` and `location` properties in the example.

## Resource Type

A resource type itself (`Microsoft.Network/dnszones@2018-05-01` in our example) consists of the following components:

- An optional provider prefix. The default provider is Azure Resource Manager whose prefix is `az://`
- A provider-specific resource type. In the case of the `az` provider, the resource type is of the form: `<ARM resource provider namespace>/<ARM resource type(s)>@<ARM API version>`.

Common `az` resource types include:

- `Microsoft.Compute/virtualMachineScaleSets@2018-10-01`
- `Microsoft.Network/virtualNetworks/subnets@2018-11-01`
- `Microsoft.Authorization/roleAssignments@2018-09-01-preview`

A full `resource` declaration with looks like the following:

```bicep
resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = {
  name: 'myZone'
  location: 'global'
}
```

## Child resources
ARM resources have an inbuilt hierarchy, and it is common for resources to have 'child resources'. In Bicep you have a few syntactic options for representing this relationship.

### Parent property
You can declare the child resource side-by-side with the parent resource, using the `parent` property to express the hierarchical relationship. You simply need to set the property value to the symbolic name of the parent resource.

```bicep
resource myParent 'My.Rp/parentType@2020-01-01' = {
  name: 'myParent'
  ...
}

resource myChild 'My.Rp/parentType/childType@2020-01-01' = {
  parent: myParent
  name: 'myChild'
  ...
}

output childProp string = myChild.properties.someProp
```

If you need to deploy a child resource without deploying the parent, you may use the `parent` property in conjunction with [existing](#referencing-existing-resources):
```bicep
resource myParent 'My.Rp/parentType@2020-01-01' existing = {
  name: 'myParent'
}

resource myChild 'My.Rp/parentType/childType@2020-01-01' = {
  parent: myParent
  name: 'myChild'
  ...
}
```

> **NOTE:** Unlike deploying child resources in ARM JSON Templates, you **must only** supply the child resource name when using the `parent` property. Bicep does not require you to manually incorporate the parent resource name using `/` characters, as it captures the full resource hierarchy through the `parent` property.

### Resource nesting
A resource declaration may appear inside another resource declaration when the inner resource is a child type of the containing parent resource. A nested resource is considered to have an implicit dependency on its containing resource for creation order.

```bicep
resource myParent 'My.Rp/parentType@2020-01-01' = {
  name: 'myParent'
  location: 'West US'

  // declares a resource of type 'My.Rp/parentType/childType@2020-01-01' 
  resource myChild 'childType' = {
    name: 'myChild'
    properties: {
      displayName: 'child in ${myParent.location}'
    }
  }
}
```

A nested resource must specify a single type segment to declare its type. The full type of the nested resource is the containing resource's type with the additional segment appended to the list of types. In the example above `My.Rp/parentType@2020-01-01` is combined with `childType` resulting in `My.Rp/parentType/childType@2020-01-01`. The nested resource may optionally declare an API version using the syntax `<segment>@<version>`. If the nested resource omits the API version the the API version of the containing resource is used. If the nested resource specifies an API version then the API version specified will be used. If the example above were modified so that the nested resource declared its type as `childType@2020-20-20` then the fully-qualified type would be `My.Rp/parentType/childType@2020-20-20`.

Nested resource declarations should specify their `name` property with a single segment. The example above the nested resource declares its `name` property with the value `myChild`. Note that this is in contrast to ARM Template JSON, where child resources must declare their `name` property as a `/`-separated string containing multiple segments like: `myParent/myChild` - this is **not** required with nested resources.

A nested resource declaration must appear at the top level of syntax of the containing resource. Declarations may be nested arbitrarily deep, as long as each level is a child type of its containing resource.

To access the child resource symbolic name, you need to use the `::` operator. For example, if you need to output a property from the child you write the following:

```bicep
output childProp string = myParent::myChild.properties.displayName
```

A nested resource may access properties of its parent resource. Other resources declared inside the body of the same containing resource may reference each other and the typical rules about cyclic-dependencies apply. A containing resource may not access properties of the resources it contains, this would cause a cyclic-dependency.

More info on the nested child resource access operator can be found in the [expressions spec](./expressions.md#nested-resource-accessors).

### Manually building the name property
Another option is to supply all of the hierarchical name information manually in the resource `name` property, by using `/` to format the name.

> **NOTE:** This approach is more error-prone, reduces Bicep's ability to understand relationships between resources as well, and can reduce some of the type validation we are able to perform. We generally recommend instead using either the ['parent property'](#parent-property) or ['nested resource'](#resource-nesting) approach when authoring to avoid these problems.

In the following example, we use `myParent.name` to allow Bicep to detect the relationship between parent & child resource - see [Resource dependencies](#resource-dependencies) for more information on how this works and why it is necessary:
```bicep
resource myParent 'My.Rp/parentType@2020-01-01' = {
  name: 'myParent'
  ...
}

resource myChild 'My.Rp/parentType/childType@2020-01-01' = {
  name: '${myParent.name}/myChild'
  ...
}

output childProp string = myChild.properties.someProp
```

If you do not wish to deploy the parent resource in the same file, the following is also valid:
```bicep
resource myChild 'My.Rp/parentType/childType@2020-01-01' = {
  name: 'myParent/myChild'
  ...
}

output childProp string = myChild.properties.someProp
```

## Resource dependencies

All declared resources will be deployed concurrently in the compiled template. Order of resource deployment can be influenced in the following ways:

### Implicit dependency

An implicit dependency will be created when one resource declaration references the identifier of another resource declaration in an expression. Here's an example:

```bicep
resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = {
  name: 'myZone'
  location: 'global'
}

resource otherResource 'Microsoft.Example/examples@2020-06-01' = {
  name: 'exampleResource'
  properties: {
    // get read-only DNS zone property
    nameServers: dnsZone.properties.nameServers
  }
}
```

A nested resource also has an implicit dependency on its containing resource.

```bicep
resource myParent 'My.Rp/parentType@2020-01-01' = {
  name: 'myParent'
  location: 'West US'

  // depends on 'myParent' implicitly
  resource myChild 'childType' = {
    name: 'myChild'
  }
}
```

### Explicit dependency

An explicit dependency is declared via the `dependsOn` property within the resource declaration. The property accept an array of resource identifiers. Here is an example of one DNS zone depending on another explicitly:

```bicep
resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = {
  name: 'myZone'
  location: 'global'
}

resource otherZone 'Microsoft.Network/dnszones@2018-05-01' = {
  name: 'myZone'
  location: 'global'
  dependsOn: [
    dnsZone
  ]
}
```

**Note:** While explicit dependencies are sometimes required, the need for them is rare. In most cases you will have a symbolic reference available to imply the dependency between resources. If you find yourself using `dependsOn` you should consider if there is a way to get rid of it.

## Conditions & Loops

### Conditions
Resources may be deployed if and only if a specified condition evaluated to `true`. Otherwise, resource deployment will be skipped. This is accomplished by adding a `if` keyword and a boolean expression to the resource declaration. The template compiled from the below example will deploy the DNS zone if the `deployZone` parameter evaluates to `true`:

```bicep
param deployZone bool

resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = if (deployZone) {
  name: 'myZone'
  location: 'global'
}
```

Conditions may be used with dependency declarations. If the identifier of conditional resource is specified in `dependsOn` of another resource (explicit dependency), the dependency will be ignored if the condition evaluates to `false` at template deployment time. If the condition evaluates to `true`, the dependency will be respected. Referencing a property of a conditional resource (implicit dependency) is allowed but may produce a runtime error in some cases.

### Loops
See the [loops](./loops.md) spec for information on deploying resources in a loop. Note that the loop & condition syntaxes can be used together on a single resource.

## Referencing existing resources
You may use the `existing` keyword to obtain symbolic reference to a resource which you know already exists. This allows you to access runtime properties from the resource without actually triggering it to be deployed.

When using the `existing` keyword, you must provide the `name` of the resource, and may optionally also set the `scope` property to access a resource in a different scope. See [Resource Scopes](./resource-scopes.md) for more information on using the `scope` property.

```bicep
// this resource will not be deployed by this file, but the declaration provides access to properties on the existing resource.
resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' existing = {
  name: 'myacc'
}

// the 'stg' symbolic name may now be used to access properties on the storage account.
output blobEndpoint string = stg.properties.primaryEndpoints.blob
```

```bicep
// example of referencing a resource at a different scope (resource group myRg under subscription mySub)
resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' existing = {
  name: 'myacc'
  scope: resourceGroup(mySub, myRg)
}
```

> **NOTE:** For those familiar with ARM JSON Templates, the `existing` keyword provides similar functionality to the `reference()` function.

## Using `list` functions on a resource
> Requires Bicep v0.4.412 or later

A number of resources provide additional functions for accessing read-only, or sensitive information, using the prefix `list` by convention. Bicep allows you to access these functions with the `.` operator on a resource reference - e.g. `ref.listSomething()`.

```bicep
resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' existing = {
  name: 'myacc'
  scope: resourceGroup('otherRg')
}

// defaults the API version for listKeys to that of the resource ('2019-06-01')
var primaryKey = stg.listKeys().keys[0].value

// use a custom API version for listKeys
var primaryKey = stg.listKeys('2020-01-01').keys[0].value

// Supply data to the list* function
var someData = {
  foo: 'bar'
}
var primaryKey = stg.listKeys('2020-01-01', someData).keys[0].value
```

> **NOTE:** Bicep does not currently have completions and validation available for `list*` functions. Some documentation is available [here](https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/template-functions-resource#list) on what is supported in ARM JSON. We are tracking support for type validation & completions under issue [#667](https://github.com/Azure/bicep/issues/667).

## Other Examples

### Storage Account

```bicep
resource myStorageAccount `Microsoft.Storage/storageAccounts@2017-10-01` = {
  name: storageAccountName
  location: resourceGroup().location
  properties: {
    supportsHttpsTrafficOnly: true
    accessTier: 'Hot'
    encryption: {
      keySource: 'Microsoft.Storage'
      services: {
        blob: {
          enabled: true
        }
        file: {
          enabled: true
        }
      }
    }
  }
  kind: StorageV2
  sku: {
    name: 'Standard_LRS'
  }
}
```
