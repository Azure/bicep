// Desired State Configuration PowerShell script for a Windows Virtual Machine
resource virtualMachine 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: /*${1:'name'}*/'name'
  location: /*${2:location}*/'location'
}

resource /*${3:windowsVMDsc}*/windowsVMDsc 'Microsoft.Compute/virtualMachines/extensions@2020-12-01' = {
  parent: virtualMachine
  name: /*${4:'name'}*/'name'
  location: /*${2:location}*/'location'
  properties: {
    publisher: 'Microsoft.Powershell'
    type: 'DSC'
    typeHandlerVersion: '2.9'
    autoUpgradeMinorVersion: true
    settings: {
      modulesUrl: /*${5:'modulesUrl'}*/'modulesUrl'
      sasToken: /*${6:'sasToken'}*/'sasToken'
      configurationFunction: /*${7:'configurationFunction'}*/'configurationFunction'
    }
  }
}
