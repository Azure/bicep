param virtualMachine2Name string

var virtualMachineName = 'VM-MultiNic'

resource virtualMachine 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  name: virtualMachineName
  location: 'westus'
//@[12:20) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'westus' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'westus'|
}

resource virtualMachine2 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  name: virtualMachine2Name
  location: 'westus'
//@[12:20) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'westus' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'westus'|
}
