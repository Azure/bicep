targetScope = 'subscription'

param prefix string = 'majastrz'
//@[06:12) Parameter prefix. Type: string. Declaration start char: 0, length: 32
var groups = [
//@[04:10) Variable groups. Type: ['bicep1', 'bicep2', 'bicep3', 'bicep4']. Declaration start char: 0, length: 60
  'bicep1'
  'bicep2'
  'bicep3'
  'bicep4'
]

var scripts = take(groups, 2)
//@[04:11) Variable scripts. Type: array. Declaration start char: 0, length: 29

resource resourceGroups 'Microsoft.Resources/resourceGroups@2020-06-01' = [for name in groups: {
//@[79:83) Local name. Type: 'bicep1' | 'bicep2' | 'bicep3' | 'bicep4'. Declaration start char: 79, length: 4
//@[09:23) Resource resourceGroups. Type: Microsoft.Resources/resourceGroups@2020-06-01[]. Declaration start char: 0, length: 148
  name: '${prefix}-${name}'
  location: 'westus'
}]

module scopedToSymbolicName 'hello.bicep' = [for (name, i) in scripts: {
//@[50:54) Local name. Type: any. Declaration start char: 50, length: 4
//@[56:57) Local i. Type: int. Declaration start char: 56, length: 1
//@[07:27) Module scopedToSymbolicName. Type: module[]. Declaration start char: 0, length: 183
  name: '${prefix}-dep-${i}'
  params: {
    scriptName: 'test-${name}-${i}'
  }
  scope: resourceGroups[i]
}]

module scopedToResourceGroupFunction 'hello.bicep' = [for (name, i) in scripts: {
//@[59:63) Local name. Type: any. Declaration start char: 59, length: 4
//@[65:66) Local i. Type: int. Declaration start char: 65, length: 1
//@[07:36) Module scopedToResourceGroupFunction. Type: module[]. Declaration start char: 0, length: 212
  name: '${prefix}-dep-${i}'
  params: {
    scriptName: 'test-${name}-${i}'
  }
  scope: resourceGroup(concat(name, '-extra'))
}]


