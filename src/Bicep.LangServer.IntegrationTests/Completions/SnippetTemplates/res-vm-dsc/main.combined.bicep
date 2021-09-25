// $1 = 'name'
// $2 = windowsVMDsc
// $3 = 'name'
// $4 = 'modulesUrl'
// $5 = 'sasToken'
// $6 = 'configurationFunction'

resource virtualMachine 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'name'
  location: resourceGroup().location
}

resource windowsVMDsc 'Microsoft.Compute/virtualMachines/extensions@2020-12-01' = {
  parent: virtualMachine
  name: 'name'
  location: resourceGroup().location
  properties: {
    publisher: 'Microsoft.Powershell'
    type: 'DSC'
    typeHandlerVersion: '2.9'
    autoUpgradeMinorVersion: true
    settings: {
      modulesUrl: 'modulesUrl'
      sasToken: 'sasToken'
      configurationFunction: 'configurationFunction'
    }
  }
}
// Insert snippet here

