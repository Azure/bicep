// Desired State Configuration PowerShell script for a Windows Virtual Machine
resource virtualMachine 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: ${1:'name'}
  location: resourceGroup().location
}

resource ${2:windowsVMDsc} 'Microsoft.Compute/virtualMachines/extensions@2020-12-01' = {
  parent: virtualMachine
  name: ${3:'name'}
  location: resourceGroup().location
  properties: {
    publisher: 'Microsoft.Powershell'
    type: 'DSC'
    typeHandlerVersion: '2.9'
    autoUpgradeMinorVersion: true
    settings: {
      modulesUrl: ${4:'modulesUrl'}
      sasToken: ${5:'sasToken'}
      configurationFunction: ${6:'configurationFunction'}
    }
  }
}
