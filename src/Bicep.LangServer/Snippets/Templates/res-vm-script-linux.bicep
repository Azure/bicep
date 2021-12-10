// Custom script extension for Linux Virtual Machine
resource virtualMachine 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: /*${1:'name'}*/'name'
  location: /*${2:location}*/'location'
}

resource /*${3:linuxVMExtensions}*/linuxVMExtensions 'Microsoft.Compute/virtualMachines/extensions@2019-07-01' = {
  parent: virtualMachine
  name: /*${4:'name'}*/'name'
  location: /*${2:location}*/'location'
  properties: {
    publisher: 'Microsoft.Azure.Extensions'
    type: 'CustomScript'
    typeHandlerVersion: '2.1'
    autoUpgradeMinorVersion: true
    settings: {
      fileUris: [
        /*${5:'fileUris'}*/'fileUris'
      ]
    }
    protectedSettings: {
      commandToExecute: /*'sh ${6:customScript.sh}'*/'sh customScript.sh'
    }
  }
}
