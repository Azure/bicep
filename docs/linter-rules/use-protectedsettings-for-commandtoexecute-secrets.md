# Use protectedSettings for commandToExecute secrets

**Code**: use-protectedsettings-for-commandtoexecute-secrets

**Description**: For custom script resources (where properties.type contains "CustomScript"), commandToExecute should be placed under the protectedSettings object if it includes secret data such as a password. For example, secret data can be used in secure parameters of type secureString or secureObject, list* functions such as listKeys, or custom scripts.

Don't use secret data in the settings object because it uses clear text. For more information, see [Microsoft.Compute virtualMachines/extensions](https://docs.microsoft.com/en-us/azure/templates/microsoft.compute/virtualmachines/extensions), [Custom Script Extension for Windows](https://docs.microsoft.com/en-us/azure/virtual-machines/extensions/custom-script-windows), and [Use the Azure Custom Script Extension Version 2 with Linux virtual machines](https://docs.microsoft.com/en-us/azure/virtual-machines/extensions/custom-script-linux).

The following example fails because commandToExecute is specified under settings and uses a secure parameter.
```bicep
param vmName string
param location string
param fileUris string

@secure()
param arguments string

var UriFileNamePieces = split(fileUris, '/')
var firstFileNameString = UriFileNamePieces[(length(UriFileNamePieces) - 1)]
var firstFileNameBreakString = split(firstFileNameString, '?')
var firstFileName = firstFileNameBreakString[0]

resource customScriptExtension 'Microsoft.Compute/virtualMachines/extensions@2021-04-01' = {
  name: '${vmName}/CustomScriptExtension'
  location: location
  properties: {
    publisher: 'Microsoft.Compute'
    type: 'CustomScriptExtension'
    autoUpgradeMinorVersion: true
    settings: {
      fileUris: split(fileUris, ' ')
      commandToExecute: 'powershell -ExecutionPolicy Unrestricted -File ${firstFileName} ${arguments}'
    }
  }
}
```

The following example fails because settings uses commandToExecute with a listKeys function.
```bicep
resource customScriptExtension 'Microsoft.HybridCompute/machines/extensions@2019-08-02-preview' = {
  name: '${vmName}/CustomScriptExtension'
  location: location
  properties: {
    publisher: 'Microsoft.Compute'
    type: 'CustomScriptExtension'
    autoUpgradeMinorVersion: true
    settings: {
      fileUris: split(fileUris, ' ')
      commandToExecute: '<command> ${storageAccountId.listKeys().keys[0].value}'
    }
  }
}
```

The following example passes because commandToExecute uses a secure parameter but is defined under protectedSettings
```bicep
param vmName string
param location string
param fileUris string

@secure()
param arguments string

var UriFileNamePieces = split(fileUris, '/')
var firstFileNameString = UriFileNamePieces[(length(UriFileNamePieces) - 1)]
var firstFileNameBreakString = split(firstFileNameString, '?')
var firstFileName = firstFileNameBreakString[0]

resource customScriptExtension 'Microsoft.Compute/virtualMachines/extensions@2021-04-01' = {
  name: '${vmName}/CustomScriptExtension'
  location: location
  properties: {
    publisher: 'Microsoft.Compute'
    type: 'CustomScriptExtension'
    autoUpgradeMinorVersion: true
    settings: {
      fileUris: split(fileUris, ' ')
    }
    protectedSettings: {
      commandToExecute: 'powershell -ExecutionPolicy Unrestricted -File ${firstFileName} ${arguments}'
    }
  }
}
```

The following example passes because protectedSettings uses a listKeys function but is defined under protectedSettings
```bicep
resource customScriptExtension 'Microsoft.HybridCompute/machines/extensions@2019-08-02-preview' = {
  name: '${vmName}/CustomScriptExtension'
  location: location
  properties: {
    publisher: 'Microsoft.Compute'
    type: 'CustomScriptExtension'
    autoUpgradeMinorVersion: true
    settings: {
      fileUris: split(fileUris, ' ')
    }
    protectedSettings: {
      commandToExecute: '<command> ${storageAccountId.listKeys().keys[0].value}'
    }
  }
}
```
