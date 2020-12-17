# Resources
A `resource` declaration defines a resource that will be either created or updated at deployment time along with its intended state. The resource is also assigned to an identifier. You can reference the identifier in [expressions](./expressions.md) that are part of [variables](./variables.md), [outputs](./outputs), or other `resource` declarations.

Consider the following declaration that creates or updates a [DNS Zone](https://docs.microsoft.com/en-us/azure/dns/dns-zones-records):
```
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

Their fully qualified equivalents would be:
- `az://Microsoft.Compute/virtualMachineScaleSets@2018-10-01`
- `az://Microsoft.Network/virtualNetworks/subnets@2018-11-01`
- `az://Microsoft.Authorization/roleAssignments@2018-09-01-preview`

> **Note**: Provider prefixes such as `az://` are not yet implemented.

A full `resource` declaration with a fully qualified resource type looks like the following:
```
resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = {
  name: 'myZone'
  location: 'global'
}
```

## Resource dependencies
All declared resources will be deployed concurrently in the compiled template. Order of resource deployment can be influenced in the following ways:
### Explicit dependency
An explicit dependency is declared via the `dependsOn` property within the resource declaration. The property accept an array of resource identifiers. Here is an example of one DNS zone depending on another explicitly:
```
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

### Implicit dependency
An implicit dependency will be created when one resource declaration references the identifier of another resource declaration in an expression. Here's an example:
```
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

## Conditions
Resources may be deployed if and only if a specified condition evaluated to `true`. Otherwise, resource deployment will be skipped. This is accomplished by adding a `if` keyword and a boolean expression to the resource declaration. The template compiled from the below example will deploy the DNS zone if the `deployZone` parameter evaluates to `true`:
```
param deployZone bool

resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = if (deployZone) {
  name: 'myZone'
  location: 'global'
}
```

Conditions may be used with dependency declarations. If the identifier of conditional resource is specified in `dependsOn` of another resource (explicit dependency), the dependency will be ignored if the condition evaluates to `false` at template deployment time. If the condition evaluates to `true`, the dependency will be respected. Referencing a property of a conditional resource (implicit dependency) is allowed but may produce a runtime error in some cases.

## Other Examples

### Storage Account
```
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
