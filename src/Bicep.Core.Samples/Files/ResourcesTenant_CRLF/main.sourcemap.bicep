targetScope = 'tenant'

var managementGroups = [
//@[line02->line11]     "managementGroups": [
//@[line02->line20]     ]
  {
//@[line03->line12]       {
//@[line03->line15]       },
    name: 'one'
//@[line04->line13]         "name": "one",
    displayName: 'The first'
//@[line05->line14]         "displayName": "The first"
  }
  {
//@[line07->line16]       {
//@[line07->line19]       }
    name: 'two'
//@[line08->line17]         "name": "two",
    displayName: 'The second'
//@[line09->line18]         "displayName": "The second"
  }
]

resource singleGroup 'Microsoft.Management/managementGroups@2020-05-01' = {
//@[line13->line23]     {
//@[line13->line24]       "type": "Microsoft.Management/managementGroups",
//@[line13->line25]       "apiVersion": "2020-05-01",
//@[line13->line26]       "name": "myMG",
//@[line13->line30]     },
  name: 'myMG'
  properties: {
//@[line15->line27]       "properties": {
//@[line15->line29]       }
    displayName: 'This one is mine!'
//@[line16->line28]         "displayName": "This one is mine!"
  }
}

resource manyGroups 'Microsoft.Management/managementGroups@2020-05-01' = [for mg in managementGroups: {
//@[line20->line31]     {
//@[line20->line32]       "copy": {
//@[line20->line33]         "name": "manyGroups",
//@[line20->line34]         "count": "[length(variables('managementGroups'))]"
//@[line20->line35]       },
//@[line20->line36]       "type": "Microsoft.Management/managementGroups",
//@[line20->line37]       "apiVersion": "2020-05-01",
//@[line20->line38]       "name": "[variables('managementGroups')[copyIndex()].name]",
//@[line20->line42]       "dependsOn": [
//@[line20->line43]         "[tenantResourceId('Microsoft.Management/managementGroups', 'myMG')]"
//@[line20->line44]       ]
//@[line20->line45]     },
  name: mg.name
  properties: {
//@[line22->line39]       "properties": {
//@[line22->line41]       },
    displayName: '${mg.displayName} (${singleGroup.properties.displayName})'
//@[line23->line40]         "displayName": "[format('{0} ({1})', variables('managementGroups')[copyIndex()].displayName, reference(tenantResourceId('Microsoft.Management/managementGroups', 'myMG'), '2020-05-01').displayName)]"
  }
}]

resource anotherSet 'Microsoft.Management/managementGroups@2020-05-01' = [for (mg, index) in managementGroups: {
//@[line27->line46]     {
//@[line27->line47]       "copy": {
//@[line27->line48]         "name": "anotherSet",
//@[line27->line49]         "count": "[length(variables('managementGroups'))]"
//@[line27->line50]       },
//@[line27->line51]       "type": "Microsoft.Management/managementGroups",
//@[line27->line52]       "apiVersion": "2020-05-01",
//@[line27->line53]       "name": "[concat(variables('managementGroups')[copyIndex()].name, '-one-', copyIndex())]",
//@[line27->line57]       "dependsOn": [
//@[line27->line58]         "manyGroups",
//@[line27->line59]         "[tenantResourceId('Microsoft.Management/managementGroups', 'myMG')]"
//@[line27->line60]       ]
//@[line27->line61]     },
  name: concat(mg.name, '-one-', index)
  properties: {
//@[line29->line54]       "properties": {
//@[line29->line56]       },
    displayName: '${mg.displayName} (${singleGroup.properties.displayName}) (set 1) (index ${index})'
//@[line30->line55]         "displayName": "[format('{0} ({1}) (set 1) (index {2})', variables('managementGroups')[copyIndex()].displayName, reference(tenantResourceId('Microsoft.Management/managementGroups', 'myMG'), '2020-05-01').displayName, copyIndex())]"
  }
  dependsOn: [
    manyGroups
  ]
}]

resource yetAnotherSet 'Microsoft.Management/managementGroups@2020-05-01' = [for mg in managementGroups: {
//@[line37->line62]     {
//@[line37->line63]       "copy": {
//@[line37->line64]         "name": "yetAnotherSet",
//@[line37->line65]         "count": "[length(variables('managementGroups'))]"
//@[line37->line66]       },
//@[line37->line67]       "type": "Microsoft.Management/managementGroups",
//@[line37->line68]       "apiVersion": "2020-05-01",
//@[line37->line69]       "name": "[concat(variables('managementGroups')[copyIndex()].name, '-two')]",
//@[line37->line73]       "dependsOn": [
//@[line37->line74]         "[tenantResourceId('Microsoft.Management/managementGroups', concat(variables('managementGroups')[0].name, '-one-', 0))]",
//@[line37->line75]         "[tenantResourceId('Microsoft.Management/managementGroups', 'myMG')]"
//@[line37->line76]       ]
//@[line37->line77]     }
  name: concat(mg.name, '-two')
  properties: {
//@[line39->line70]       "properties": {
//@[line39->line72]       },
    displayName: '${mg.displayName} (${singleGroup.properties.displayName}) (set 2)'
//@[line40->line71]         "displayName": "[format('{0} ({1}) (set 2)', variables('managementGroups')[copyIndex()].displayName, reference(tenantResourceId('Microsoft.Management/managementGroups', 'myMG'), '2020-05-01').displayName)]"
  }
  dependsOn: [
    anotherSet[0]
  ]
}]

output managementGroupIds array = [for i in range(0, length(managementGroups)): {
//@[line47->line80]     "managementGroupIds": {
//@[line47->line81]       "type": "array",
//@[line47->line82]       "copy": {
//@[line47->line83]         "count": "[length(range(0, length(variables('managementGroups'))))]",
//@[line47->line84]         "input": {
//@[line47->line87]         }
//@[line47->line88]       }
//@[line47->line89]     }
  name: yetAnotherSet[i].name
//@[line48->line85]           "name": "[concat(variables('managementGroups')[range(0, length(variables('managementGroups')))[copyIndex()]].name, '-two')]",
  displayName: yetAnotherSet[i].properties.displayName
//@[line49->line86]           "displayName": "[reference(tenantResourceId('Microsoft.Management/managementGroups', concat(variables('managementGroups')[range(0, length(variables('managementGroups')))[copyIndex()]].name, '-two')), '2020-05-01').displayName]"
}]

