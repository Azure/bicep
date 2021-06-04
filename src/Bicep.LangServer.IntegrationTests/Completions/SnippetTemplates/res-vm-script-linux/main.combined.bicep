// $1 = 'name'
// $2 = linuxVMExtensions
// $3 = 'name'
// $4 = 'fileUris'
// $5 = customScript.sh

resource virtualMachine 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'name'
  location: resourceGroup().location
}

resource linuxVMExtensions 'Microsoft.Compute/virtualMachines/extensions@2019-07-01' = {
  parent: virtualMachine
  name: 'name'
  location: resourceGroup().location
  properties: {
    publisher: 'Microsoft.Azure.Extensions'
    type: 'CustomScript'
    typeHandlerVersion: '2.1'
    autoUpgradeMinorVersion: true
    settings: {
      fileUris: [
        'fileUris'
      ]
    }
    protectedSettings: {
      commandToExecute: 'sh customScript.sh'
    }
  }
}
// Insert snippet here

