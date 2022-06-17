targetScope = 'tenant'

var managementGroups = [
//@[12:21]     "managementGroups": [
  {
    name: 'one'
//@[14:14]         "name": "one",
    displayName: 'The first'
//@[15:15]         "displayName": "The first"
  }
  {
    name: 'two'
//@[18:18]         "name": "two",
    displayName: 'The second'
//@[19:19]         "displayName": "The second"
  }
]

resource singleGroup 'Microsoft.Management/managementGroups@2020-05-01' = {
//@[24:31]       "type": "Microsoft.Management/managementGroups",
  name: 'myMG'
  properties: {
//@[28:30]       "properties": {
    displayName: 'This one is mine!'
//@[29:29]         "displayName": "This one is mine!"
  }
}

resource manyGroups 'Microsoft.Management/managementGroups@2020-05-01' = [for mg in managementGroups: {
//@[32:46]       "copy": {
  name: mg.name
  properties: {
//@[40:42]       "properties": {
    displayName: '${mg.displayName} (${singleGroup.properties.displayName})'
//@[41:41]         "displayName": "[format('{0} ({1})', variables('managementGroups')[copyIndex()].displayName, reference(tenantResourceId('Microsoft.Management/managementGroups', 'myMG')).displayName)]"
  }
}]

resource anotherSet 'Microsoft.Management/managementGroups@2020-05-01' = [for (mg, index) in managementGroups: {
//@[47:62]       "copy": {
  name: concat(mg.name, '-one-', index)
  properties: {
//@[55:57]       "properties": {
    displayName: '${mg.displayName} (${singleGroup.properties.displayName}) (set 1) (index ${index})'
//@[56:56]         "displayName": "[format('{0} ({1}) (set 1) (index {2})', variables('managementGroups')[copyIndex()].displayName, reference(tenantResourceId('Microsoft.Management/managementGroups', 'myMG')).displayName, copyIndex())]"
  }
  dependsOn: [
    manyGroups
  ]
}]

resource yetAnotherSet 'Microsoft.Management/managementGroups@2020-05-01' = [for mg in managementGroups: {
//@[63:78]       "copy": {
  name: concat(mg.name, '-two')
  properties: {
//@[71:73]       "properties": {
    displayName: '${mg.displayName} (${singleGroup.properties.displayName}) (set 2)'
//@[72:72]         "displayName": "[format('{0} ({1}) (set 2)', variables('managementGroups')[copyIndex()].displayName, reference(tenantResourceId('Microsoft.Management/managementGroups', 'myMG')).displayName)]"
  }
  dependsOn: [
    anotherSet[0]
  ]
}]

output managementGroupIds array = [for i in range(0, length(managementGroups)): {
//@[81:90]     "managementGroupIds": {
  name: yetAnotherSet[i].name
//@[86:86]           "name": "[concat(variables('managementGroups')[range(0, length(variables('managementGroups')))[copyIndex()]].name, '-two')]",
  displayName: yetAnotherSet[i].properties.displayName
//@[87:87]           "displayName": "[reference(tenantResourceId('Microsoft.Management/managementGroups', concat(variables('managementGroups')[range(0, length(variables('managementGroups')))[copyIndex()]].name, '-two'))).displayName]"
}]

