param virtualMachine2Name string
param location string

var virtualMachineName = 'VM-MultiNic'
var virtualMachineRename_var = 'VM-MultiNic'
//@[4:28) [decompiler-cleanup (Warning)] The name of variable 'virtualMachineRename_var' appears to have originated from a naming conflict during a decompilation from JSON. Consider renaming it and removing the suffix (using the editor's rename functionality). (bicep core linter https://aka.ms/bicep/linter-diagnostics#decompiler-cleanup) |virtualMachineRename_var|
var virtualMachineName3 = 'VM-MultiNic'

resource virtualMachine 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  name: virtualMachineName
//@[8:26) [BCP121 (Error)] Resources: "virtualMachine", "virtualMachineRename", "virtualMachine3" are defined with this same name in a file. Rename them or split into different modules. (bicep https://aka.ms/bicep/core-diagnostics#BCP121) |virtualMachineName|
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
//@[8:32) [BCP121 (Error)] Resources: "virtualMachine", "virtualMachineRename", "virtualMachine3" are defined with this same name in a file. Rename them or split into different modules. (bicep https://aka.ms/bicep/core-diagnostics#BCP121) |virtualMachineRename_var|
  location: location
}

resource virtualMachine3 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  name: virtualMachineName3
//@[8:27) [BCP121 (Error)] Resources: "virtualMachine", "virtualMachineRename", "virtualMachine3" are defined with this same name in a file. Rename them or split into different modules. (bicep https://aka.ms/bicep/core-diagnostics#BCP121) |virtualMachineName3|
  location: location
}

