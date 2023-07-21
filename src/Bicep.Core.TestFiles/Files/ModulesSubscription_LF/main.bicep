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
}]

