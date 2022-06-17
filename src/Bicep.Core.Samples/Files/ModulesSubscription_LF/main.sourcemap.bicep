targetScope = 'subscription'

param prefix string = 'majastrz'
//@[12:15]     "prefix": {
var groups = [
//@[18:23]     "groups": [
  'bicep1'
//@[19:19]       "bicep1",
  'bicep2'
//@[20:20]       "bicep2",
  'bicep3'
//@[21:21]       "bicep3",
  'bicep4'
//@[22:22]       "bicep4"
]

var scripts = take(groups, 2)
//@[24:24]     "scripts": "[take(variables('groups'), 2)]"

resource resourceGroups 'Microsoft.Resources/resourceGroups@2020-06-01' = [for name in groups: {
//@[27:36]       "copy": {
  name: '${prefix}-${name}'
  location: 'westus'
//@[35:35]       "location": "westus"
}]

module scopedToSymbolicName 'hello.bicep' = [for (name, i) in scripts: {
//@[37:96]       "copy": {
  name: '${prefix}-dep-${i}'
//@[44:44]       "name": "[format('{0}-dep-{1}', parameters('prefix'), copyIndex())]",
  params: {
    scriptName: 'test-${name}-${i}'
  }
  scope: resourceGroups[i]
}]

module scopedToResourceGroupFunction 'hello.bicep' = [for (name, i) in scripts: {
//@[97:153]       "copy": {
  name: '${prefix}-dep-${i}'
//@[104:104]       "name": "[format('{0}-dep-{1}', parameters('prefix'), copyIndex())]",
  params: {
    scriptName: 'test-${name}-${i}'
  }
  scope: resourceGroup(concat(name, '-extra'))
}]


