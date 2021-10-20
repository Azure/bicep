targetScope = 'subscription'

param prefix string = 'majastrz'
var groups = [
  'bicep1'
  'bicep2'
  'bicep3'
  'bicep4'
]

var scripts = take(groups, 2)

resource resourceGroups 'Microsoft.Resources/resourceGroups@2020-06-01' = [for name in groups: {
  name: '${prefix}-${name}'
  location: 'westus'
//@[12:20) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'westus' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'westus'|
}]

module scopedToSymbolicName 'hello.bicep' = [for (name, i) in scripts: {
  name: '${prefix}-dep-${i}'
  params: {
    scriptName: 'test-${name}-${i}'
  }
  scope: resourceGroups[i]
}]

module scopedToResourceGroupFunction 'hello.bicep' = [for (name, i) in scripts: {
  name: '${prefix}-dep-${i}'
  params: {
    scriptName: 'test-${name}-${i}'
  }
  scope: resourceGroup(concat(name, '-extra'))
//@[23:45) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(name, '-extra')|
}]


