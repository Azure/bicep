targetScope = 'tenant'

var managementGroups = [
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
  name: 'myMG'
  properties: {
    displayName: 'This one is mine!'
  }
}

resource manyGroups 'Microsoft.Management/managementGroups@2020-05-01' = [for mg in managementGroups: {
  name: mg.name
  properties: {
    displayName: '${mg.displayName} (${singleGroup.properties.displayName})'
  }
}]

resource anotherSet 'Microsoft.Management/managementGroups@2020-05-01' = [for mg in managementGroups: {
  name: concat(mg.name, '-one')
  properties: {
    displayName: '${mg.displayName} (${singleGroup.properties.displayName}) (set 1)'
  }
  dependsOn: [
    manyGroups
  ]
}]

resource yetAnotherSet 'Microsoft.Management/managementGroups@2020-05-01' = [for mg in managementGroups: {
  name: concat(mg.name, '-two')
  properties: {
    displayName: '${mg.displayName} (${singleGroup.properties.displayName}) (set 2)'
  }
  dependsOn: [
    anotherSet[0]
  ]
}]
