// $1 = 'name'
// $2 = location
// $3 = windowsVMDsc
// $4 = 'name'
// $5 = 'modulesUrl'
// $6 = 'sasToken'
// $7 = 'configurationFunction'

param location string

resource virtualMachine 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'name'
  location: location
}

resource windowsVMDsc 'Microsoft.Compute/virtualMachines/extensions@2020-12-01' = {
  parent: virtualMachine
  name: 'name'
  location: location
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

