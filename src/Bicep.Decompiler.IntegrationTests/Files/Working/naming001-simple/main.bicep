param virtualMachine2Name string
param location string

var virtualMachineName = 'VM-MultiNic'
var virtualMachineRename_var = 'VM-MultiNic'
var virtualMachineName3 = 'VM-MultiNic'

resource virtualMachine 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  name: virtualMachineName
  location: location
}

resource virtualMachine2 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  name: virtualMachine2Name
  location: location
}

resource Name 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  name: 'Name'
  location: location
}

resource virtualMachineRename 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  name: virtualMachineRename_var
  location: location
}

resource virtualMachine3 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  name: virtualMachineName3
  location: location
}
