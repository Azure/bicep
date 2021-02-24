targetScope = 'tenant'

var managementGroups = [
//@[4:20) Variable managementGroups. Type: array. Declaration start char: 0, length: 142
  {
    name: 'one'
    displayName: 'The first'
  }
  {
    name: 'two'
    displayName: 'The second'
  }
]

resource singleGroup 'Microsoft.Management/managementGroups@2020-05-01' = {
//@[9:20) Resource singleGroup. Type: Microsoft.Management/managementGroups@2020-05-01. Declaration start char: 0, length: 154
  name: 'myMG'
  properties: {
    displayName: 'This one is mine!'
  }
}

resource manyGroups 'Microsoft.Management/managementGroups@2020-05-01' = [for mg in managementGroups: {
//@[78:80) Local mg. Type: any. Declaration start char: 78, length: 2
//@[9:19) Resource manyGroups. Type: Microsoft.Management/managementGroups@2020-05-01[]. Declaration start char: 0, length: 224
  name: mg.name
  properties: {
    displayName: '${mg.displayName} (${singleGroup.properties.displayName})'
  }
}]

resource anotherSet 'Microsoft.Management/managementGroups@2020-05-01' = [for mg in managementGroups: {
//@[78:80) Local mg. Type: any. Declaration start char: 78, length: 2
//@[9:19) Resource anotherSet. Type: Microsoft.Management/managementGroups@2020-05-01[]. Declaration start char: 0, length: 285
  name: concat(mg.name, '-one')
  properties: {
    displayName: '${mg.displayName} (${singleGroup.properties.displayName}) (set 1)'
  }
  dependsOn: [
    manyGroups
  ]
}]

resource yetAnotherSet 'Microsoft.Management/managementGroups@2020-05-01' = [for mg in managementGroups: {
//@[81:83) Local mg. Type: any. Declaration start char: 81, length: 2
//@[9:22) Resource yetAnotherSet. Type: Microsoft.Management/managementGroups@2020-05-01[]. Declaration start char: 0, length: 291
  name: concat(mg.name, '-two')
  properties: {
    displayName: '${mg.displayName} (${singleGroup.properties.displayName}) (set 2)'
  }
  dependsOn: [
    anotherSet[0]
  ]
}]
