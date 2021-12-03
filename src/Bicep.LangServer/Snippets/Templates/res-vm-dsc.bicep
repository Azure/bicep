// Desired State Configuration PowerShell script for a Windows Virtual Machine
resource virtualMachine 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: /*${1:'name'}*/'name'
  location: location
}

resource /*${2:windowsVMDsc}*/windowsVMDsc 'Microsoft.Compute/virtualMachines/extensions@2020-12-01' = {
  parent: virtualMachine
  name: /*${3:'name'}*/'name'
  location: location
  properties: {
    publisher: 'Microsoft.Powershell'
    type: 'DSC'
    typeHandlerVersion: '2.9'
    autoUpgradeMinorVersion: true
    settings: {
      modulesUrl: /*${4:'modulesUrl'}*/'modulesUrl'
      sasToken: /*${5:'sasToken'}*/'sasToken'
      configurationFunction: /*${6:'configurationFunction'}*/'configurationFunction'
    }
  }
}
