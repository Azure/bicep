// Custom script extension for a Windows Virtual Machine
resource virtualMachine 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: /*${1:'name'}*/'name'
  location: location
}

resource /*${2:windowsVMExtensions}*/windowsVMExtensions 'Microsoft.Compute/virtualMachines/extensions@2020-12-01' = {
  parent: virtualMachine
  name: /*${3:'name'}*/'name'
  location: location
  properties: {
    publisher: 'Microsoft.Compute'
    type: 'CustomScriptExtension'
    typeHandlerVersion: '1.10'
    autoUpgradeMinorVersion: true
    settings: {
      fileUris: [
        /*${4:'fileUris'}*/'fileUris'
      ]
    }
    protectedSettings: {
      commandToExecute: /*'powershell -ExecutionPolicy Bypass -file ${5:customScript.ps1}'*/'powershell -ExecutionPolicy Bypass -file customScript.ps1'
    }
  }
}
