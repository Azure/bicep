param virtualMachine2Name string
param location string

var virtualMachineName = 'VM-MultiNic'
var virtualMachineRename_var = 'VM-MultiNic'
//@[4:28) [decompiler-cleanup (Warning)] The name of variable 'virtualMachineRename_var' appears to have originated from a naming conflict during a decompilation from JSON. Consider renaming it and removing the suffix (using the editor's rename functionality). (CodeDescription: bicep core(https://aka.ms/bicep/linter/decompiler-cleanup)) |virtualMachineRename_var|
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
