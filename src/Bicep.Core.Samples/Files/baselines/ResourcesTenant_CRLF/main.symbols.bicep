targetScope = 'tenant'

var managementGroups = [
//@[04:20) Variable managementGroups. Type: [object, object]. Declaration start char: 0, length: 142
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
//@[09:20) Resource singleGroup. Type: Microsoft.Management/managementGroups@2020-05-01. Declaration start char: 0, length: 154
  name: 'myMG'
  properties: {
    displayName: 'This one is mine!'
  }
}

resource manyGroups 'Microsoft.Management/managementGroups@2020-05-01' = [for mg in managementGroups: {
//@[78:80) Local mg. Type: object | object. Declaration start char: 78, length: 2
//@[09:19) Resource manyGroups. Type: Microsoft.Management/managementGroups@2020-05-01[]. Declaration start char: 0, length: 224
  name: mg.name
  properties: {
    displayName: '${mg.displayName} (${singleGroup.properties.displayName})'
  }
}]

resource anotherSet 'Microsoft.Management/managementGroups@2020-05-01' = [for (mg, index) in managementGroups: {
//@[79:81) Local mg. Type: object | object. Declaration start char: 79, length: 2
//@[83:88) Local index. Type: int. Declaration start char: 83, length: 5
//@[09:19) Resource anotherSet. Type: Microsoft.Management/managementGroups@2020-05-01[]. Declaration start char: 0, length: 319
  name: concat(mg.name, '-one-', index)
  properties: {
    displayName: '${mg.displayName} (${singleGroup.properties.displayName}) (set 1) (index ${index})'
  }
  dependsOn: [
    manyGroups
  ]
}]

resource yetAnotherSet 'Microsoft.Management/managementGroups@2020-05-01' = [for mg in managementGroups: {
//@[81:83) Local mg. Type: object | object. Declaration start char: 81, length: 2
//@[09:22) Resource yetAnotherSet. Type: Microsoft.Management/managementGroups@2020-05-01[]. Declaration start char: 0, length: 291
  name: concat(mg.name, '-two')
  properties: {
    displayName: '${mg.displayName} (${singleGroup.properties.displayName}) (set 2)'
  }
  dependsOn: [
    anotherSet[0]
  ]
}]

output managementGroupIds array = [for i in range(0, length(managementGroups)): {
//@[39:40) Local i. Type: int. Declaration start char: 39, length: 1
//@[07:25) Output managementGroupIds. Type: array. Declaration start char: 0, length: 172
  name: yetAnotherSet[i].name
  displayName: yetAnotherSet[i].properties.displayName
}]

