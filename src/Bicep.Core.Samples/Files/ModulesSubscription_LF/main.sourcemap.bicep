targetScope = 'subscription'
//@[66:128]             "scriptName": {\r

param prefix string = 'majastrz'
//@[11:142]     "prefix": {\r
var groups = [
//@[17:22]     "groups": [\r
  'bicep1'
//@[18:135]       "bicep1",\r
  'bicep2'
//@[19:136]       "bicep2",\r
  'bicep3'
//@[20:141]       "bicep3",\r
  'bicep4'
//@[21:138]       "bicep4"\r
]

var scripts = take(groups, 2)
//@[23:23]     "scripts": "[take(variables('groups'), 2)]"\r

resource resourceGroups 'Microsoft.Resources/resourceGroups@2020-06-01' = [for name in groups: {
//@[26:35]       "copy": {\r
  name: '${prefix}-${name}'
  location: 'westus'
//@[34:34]       "location": "westus"\r
}]

module scopedToSymbolicName 'hello.bicep' = [for (name, i) in scripts: {
//@[36:95]       "copy": {\r
  name: '${prefix}-dep-${i}'
//@[43:43]       "name": "[format('{0}-dep-{1}', parameters('prefix'), copyIndex())]",\r
  params: {
    scriptName: 'test-${name}-${i}'
  }
  scope: resourceGroups[i]
}]

module scopedToResourceGroupFunction 'hello.bicep' = [for (name, i) in scripts: {
//@[96:152]       "copy": {\r
  name: '${prefix}-dep-${i}'
//@[103:103]       "name": "[format('{0}-dep-{1}', parameters('prefix'), copyIndex())]",\r
  params: {
    scriptName: 'test-${name}-${i}'
  }
  scope: resourceGroup(concat(name, '-extra'))
}]


