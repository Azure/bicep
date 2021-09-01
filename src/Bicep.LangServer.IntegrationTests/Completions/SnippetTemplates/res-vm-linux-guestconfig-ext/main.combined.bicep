// $1 = 'name'
// $2 = 'linuxVMGuestConfigExtension'

// Guest configuration extension for Linux virtual machine
resource virtualMachine 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: /*${1:'name'}*/'name'
  location: resourceGroup().location
  identity: {
    type:'SystemAssigned'
  }
}

resource /*${2:linuxVMGuestConfigExtension}*/linuxVMGuestConfigExtension 'Microsoft.Compute/virtualMachines/extensions@2020-12-01' = {
  parent: virtualMachine
  name: 'AzurePolicyforLinux'
  location: resourceGroup().location
  properties: {
    publisher: 'Microsoft.GuestConfiguration'
    type: 'ConfigurationforLinux'
    typeHandlerVersion: '1.0'
    autoUpgradeMinorVersion: true
    settings: {}
    protectedSettings: {}
  }
}

// Insert snippet here
