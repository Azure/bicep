// $1 = 'name'
// $2 = location
// $3 = linuxVMExtensions
// $4 = 'name'
// $5 = 'fileUris'
// $6 = customScript.sh

param location string

resource virtualMachine 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'name'
  location: location
}

resource linuxVMExtensions 'Microsoft.Compute/virtualMachines/extensions@2019-07-01' = {
  parent: virtualMachine
  name: 'name'
  location: location
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

