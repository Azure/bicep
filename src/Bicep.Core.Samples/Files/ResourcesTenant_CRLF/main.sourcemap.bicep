targetScope = 'tenant'

var managementGroups = [
//@[11:20]     "managementGroups": [
  {
    name: 'one'
//@[13:13]         "name": "one",
    displayName: 'The first'
//@[14:14]         "displayName": "The first"
  }
  {
    name: 'two'
//@[17:17]         "name": "two",
    displayName: 'The second'
//@[18:18]         "displayName": "The second"
  }
]

resource singleGroup 'Microsoft.Management/managementGroups@2020-05-01' = {
//@[23:30]       "type": "Microsoft.Management/managementGroups",
  name: 'myMG'
  properties: {
//@[27:29]       "properties": {
    displayName: 'This one is mine!'
//@[28:28]         "displayName": "This one is mine!"
  }
}

resource manyGroups 'Microsoft.Management/managementGroups@2020-05-01' = [for mg in managementGroups: {
//@[31:45]       "copy": {
  name: mg.name
  properties: {
//@[39:41]       "properties": {
    displayName: '${mg.displayName} (${singleGroup.properties.displayName})'
//@[40:40]         "displayName": "[format('{0} ({1})', variables('managementGroups')[copyIndex()].displayName, reference(tenantResourceId('Microsoft.Management/managementGroups', 'myMG'), '2020-05-01').displayName)]"
  }
}]

resource anotherSet 'Microsoft.Management/managementGroups@2020-05-01' = [for (mg, index) in managementGroups: {
//@[46:61]       "copy": {
  name: concat(mg.name, '-one-', index)
  properties: {
//@[54:56]       "properties": {
    displayName: '${mg.displayName} (${singleGroup.properties.displayName}) (set 1) (index ${index})'
//@[55:55]         "displayName": "[format('{0} ({1}) (set 1) (index {2})', variables('managementGroups')[copyIndex()].displayName, reference(tenantResourceId('Microsoft.Management/managementGroups', 'myMG'), '2020-05-01').displayName, copyIndex())]"
  }
  dependsOn: [
    manyGroups
  ]
}]

resource yetAnotherSet 'Microsoft.Management/managementGroups@2020-05-01' = [for mg in managementGroups: {
//@[62:77]       "copy": {
  name: concat(mg.name, '-two')
  properties: {
//@[70:72]       "properties": {
    displayName: '${mg.displayName} (${singleGroup.properties.displayName}) (set 2)'
//@[71:71]         "displayName": "[format('{0} ({1}) (set 2)', variables('managementGroups')[copyIndex()].displayName, reference(tenantResourceId('Microsoft.Management/managementGroups', 'myMG'), '2020-05-01').displayName)]"
  }
  dependsOn: [
    anotherSet[0]
  ]
}]

output managementGroupIds array = [for i in range(0, length(managementGroups)): {
//@[80:89]     "managementGroupIds": {
  name: yetAnotherSet[i].name
//@[85:85]           "name": "[concat(variables('managementGroups')[range(0, length(variables('managementGroups')))[copyIndex()]].name, '-two')]",
  displayName: yetAnotherSet[i].properties.displayName
//@[86:86]           "displayName": "[reference(tenantResourceId('Microsoft.Management/managementGroups', concat(variables('managementGroups')[range(0, length(variables('managementGroups')))[copyIndex()]].name, '-two')), '2020-05-01').displayName]"
}]

