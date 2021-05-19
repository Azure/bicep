// Custom script extension for Linux Virtual Machine
resource virtualMachine 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: ${1:'name'}
  location: resourceGroup().location
}

resource ${2:linuxVMExtensions} 'Microsoft.Compute/virtualMachines/extensions@2019-07-01' = {
  parent: virtualMachine
  name: ${3:'name'}
  location: resourceGroup().location
  properties: {
    publisher: 'Microsoft.Azure.Extensions'
    type: 'CustomScript'
    typeHandlerVersion: '2.1'
    autoUpgradeMinorVersion: true
    settings: {
      fileUris: [
        ${4:'fileUris'}
      ]
    }
    protectedSettings: {
      commandToExecute: 'sh ${5:customScript.sh}'
    }
  }
}
