# Modules

A module is an opaque set of one or more resources to be deployed together. It only exposes parameters and outputs as contract to other bicep files, hiding details on how internal resources are defined. This allows you to abstract away complex details of the raw resource declaration from the end user who now only needs to be concerned about the module contract. Parameters and outputs are optional.

## Defining a module

Any bicep file can be consumed as a module, so there is no specific syntax for defining a module.

Here is an example bicep file (`publicIpAddress.bicep`) that we will later consume as a module:
```bicep
// Input parameters must be specified by the module consumer
param publicIpResourceName string
param publicIpDnsLabel string = '${publicIpResourceName}-${newGuid()}'
param location string = resourceGroup().location
param dynamicAllocation bool

resource publicIp 'Microsoft.Network/publicIPAddresses@2020-06-01' = {
  name: publicIpResourceName
  location: location
  properties: {
    publicIPAllocationMethod: dynamicAllocation ? 'Dynamic' : 'Static'
    dnsSettings: {
      domainNameLabel: publicIpDnsLabel
    }
  }
}

// Set an output which can be accessed by the module consumer
output ipFqdn string = publicIp.properties.dnsSettings.fqdn
```

### Notes
* The `name` property is required when consuming a module. When Bicep generates the template IL, this field is used as the name of the nested deployment resource which is generated for the module.

## Consuming a module

To consume a module, we use the `module` keyword. The path to the module in this example is specified using a relative path (`../publicIpAddress.bicep`).

Here is an example consumption of a module. This will deploy the resource(s) defined in the module file being referenced:
```bicep
param publicIpName string = 'mypublicip'

module publicIp './publicIpAddress.bicep' = {
  name: 'publicIp'
  params: {
    publicIpResourceName: publicIpName
    dynamicAllocation: true
    // Parameters with default values may be omitted.
  }
}

// To reference module outputs
output ipFqdn string = publicIp.outputs.ipFqdn
```

### Notes
* All paths in Bicep must be specified using the forward slash (`/`) directory separator to ensure consistent compilation cross-platform. The Windows backslash (`\`) character is unsupported.