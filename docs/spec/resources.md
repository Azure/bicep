# Resources
A `resource` declaration defines a resource which will be either created or updated at deployment time, along with its intended state. Optionally, the resource can also be assigned to an identifier to reference it or its properties in [variables](./variables.md), [outputs](./outputs), or other `resource` declarations.

Consider the following declaration that creates or updates a [DNS Zone](https://docs.microsoft.com/en-us/azure/dns/dns-zones-records):
```
resource dnsZone 'Microsoft.Network/dnszones@2018-05-01': {
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
- An optional provider prefix. The default provider is Azure Resource Manager whose prefix is `azrm://`
- A provider-specific resource type. In the case of the `azrm` provider, the resource type is of the form: `<ARM resource provider namespace>/<ARM resource type(s)>@<ARM API version>`.

Common `azrm` resource types include:
- `Microsoft.Compute/virtualMachineScaleSets@2018-10-01`
- `Microsoft.Network/virtualNetworks/subnets@2018-11-01`
- `Microsoft.Authorization/roleAssignments@2018-09-01-preview`

Their fully qualified equivalents would be:
- `azrm://Microsoft.Compute/virtualMachineScaleSets@2018-10-01`
- `azrm://Microsoft.Network/virtualNetworks/subnets@2018-11-01`
- `azrm://Microsoft.Authorization/roleAssignments@2018-09-01-preview`

A full `resource` declaration with a fully qualified resource type looks like the following:
```
resource dnsZone 'Microsoft.Network/dnszones@2018-05-01': {
  name: 'myZone'
  location: 'global'
}
```

## Other Examples

### Storage Account
```
resource myStorageAccount `Microsoft.Storage/storageAccounts@2017-10-01`: {
  name: storageAccountName,
  location: resourceGroup().location,
  properties: {
    supportsHttpsTrafficOnly: true,
    accessTier: 'Hot',
    encryption: {
      keySource: 'Microsoft.Storage',
      services: {
        blob: {
          enabled: true
        },
        file: {
          enabled: true
        }
      }
    }
  },
  kind: StorageV2,
  sku: {
    name: 'Standard_LRS'
  }
}
```
