targetScope = 'subscription'

param prefix string = 'majastrz'
//@[11:14]     "prefix": {
var groups = [
//@[17:22]     "groups": [
  'bicep1'
//@[18:18]       "bicep1",
  'bicep2'
//@[19:19]       "bicep2",
  'bicep3'
//@[20:20]       "bicep3",
  'bicep4'
//@[21:21]       "bicep4"
]

var scripts = take(groups, 2)
//@[23:23]     "scripts": "[take(variables('groups'), 2)]"

resource resourceGroups 'Microsoft.Resources/resourceGroups@2020-06-01' = [for name in groups: {
//@[26:35]       "copy": {
  name: '${prefix}-${name}'
  location: 'westus'
}]

module scopedToSymbolicName 'hello.bicep' = [for (name, i) in scripts: {
//@[36:95]       "[string('copy')]": {
  name: '${prefix}-dep-${i}'
  params: {
    scriptName: 'test-${name}-${i}'
  }
  scope: resourceGroups[i]
}]

module scopedToResourceGroupFunction 'hello.bicep' = [for (name, i) in scripts: {
//@[96:152]       "[string('copy')]": {
  name: '${prefix}-dep-${i}'
  params: {
    scriptName: 'test-${name}-${i}'
  }
  scope: resourceGroup(concat(name, '-extra'))
}]


