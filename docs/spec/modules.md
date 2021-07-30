# Modules

A module is an opaque set of one or more resources to be deployed together. It only exposes parameters and outputs as contract to other Bicep files, hiding details on how internal resources are defined. This allows you to abstract away complex details of the raw resource declaration from the end user who now only needs to be concerned about the module contract. Parameters and outputs are optional.

## Defining a module

Every Bicep file can be consumed as a module, so there is no specific syntax for defining a module.

Here is an example Bicep file (`publicIpAddress.bicep`) that we will later consume as a module:
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

## Defining and configuring module scopes

It is possible to deploy across multiple scopes using the `scope` property when declaring a module. For example:

```bicep
module publicIp './publicIpAddress.bicep' = {
  name: 'publicIp'
  scope: resourceGroup('someOtherRg') // pass in a scope to a different resourceGroup
  params: {
    publicIpResourceName: publicIpName
    dynamicAllocation: true
  }
}
```

Please see [Resource Scopes](./resource-scopes.md) for more information and advanced usage.

## Using existing Key Vault's secret as input for secure string module parameter

When a module expects a `string` parameter with `secure: true` modifier, you can use existing secret from a Key Vault. To obtain the secret you need to use special method `getSecret` that can be called on a Microsoft.KeyVault/vaults resource only and can be used only with parameter with `@secure()` decorator. For example:

```bicep
// Module accepting secure string
@secure()
param myPassword string
@secure()
param mySecondPassword string
```

```bicep
param keyVaultName string
param keyVaultSubscription string
param keyVaultResourceGroup string
param secret1Name string
param secret1Version  string
param secret2Name string

resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: keyVaultName
  scope: resourceGroup(keyVaultSubscription, keyVaultResourceGroup)
}

module secretModule './secretModule.bicep' = {
  name: 'secretModule'  
  params: {
    myPassword: kv.getSecret(secret1Name, secret1Version)
    mySecondPassword: kv.getSecret(secret2Name)
  }
}
```

### Notes
* Key Vault must have `enabledForTemplateDeployment` property set to `true`
* Key Vault and secret must exist before entire deployment starts.
* Secret version is optional. It defaults to latest version if omitted.

### Additional links
 * https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/key-vault-parameter
 * https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/template-tutorial-use-key-vault
