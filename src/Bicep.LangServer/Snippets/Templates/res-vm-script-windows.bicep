// Custom script extension for a Windows Virtual Machine
resource virtualMachine 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: /*${1:'name'}*/'name'
  location: /*${2:location}*/'location'
}

resource /*${3:windowsVMExtensions}*/windowsVMExtensions 'Microsoft.Compute/virtualMachines/extensions@2020-12-01' = {
  parent: virtualMachine
  name: /*${4:'name'}*/'name'
  location: /*${2:location}*/'location'
  properties: {
    publisher: 'Microsoft.Compute'
    type: 'CustomScriptExtension'
    typeHandlerVersion: '1.10'
    autoUpgradeMinorVersion: true
    settings: {
      fileUris: [
        /*${5:'fileUris'}*/'fileUris'
      ]
    }
    protectedSettings: {
      commandToExecute: /*'powershell -ExecutionPolicy Bypass -file ${6:customScript.ps1}'*/'powershell -ExecutionPolicy Bypass -file customScript.ps1'
    }
  }
}
