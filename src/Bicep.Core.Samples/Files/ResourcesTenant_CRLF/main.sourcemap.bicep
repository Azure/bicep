targetScope = 'tenant'

var managementGroups = [
//@[11:20]     "managementGroups": [
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
//@[23:30]       "type": "Microsoft.Management/managementGroups",
  name: 'myMG'
  properties: {
    displayName: 'This one is mine!'
  }
}

resource manyGroups 'Microsoft.Management/managementGroups@2020-05-01' = [for mg in managementGroups: {
//@[31:45]       "copy": {
  name: mg.name
  properties: {
    displayName: '${mg.displayName} (${singleGroup.properties.displayName})'
  }
}]

resource anotherSet 'Microsoft.Management/managementGroups@2020-05-01' = [for (mg, index) in managementGroups: {
//@[46:61]       "copy": {
  name: concat(mg.name, '-one-', index)
  properties: {
    displayName: '${mg.displayName} (${singleGroup.properties.displayName}) (set 1) (index ${index})'
  }
  dependsOn: [
    manyGroups
  ]
}]

resource yetAnotherSet 'Microsoft.Management/managementGroups@2020-05-01' = [for mg in managementGroups: {
//@[62:77]       "copy": {
  name: concat(mg.name, '-two')
  properties: {
    displayName: '${mg.displayName} (${singleGroup.properties.displayName}) (set 2)'
  }
  dependsOn: [
    anotherSet[0]
  ]
}]

output managementGroupIds array = [for i in range(0, length(managementGroups)): {
//@[80:89]     "managementGroupIds": {
  name: yetAnotherSet[i].name
  displayName: yetAnotherSet[i].properties.displayName
}]

