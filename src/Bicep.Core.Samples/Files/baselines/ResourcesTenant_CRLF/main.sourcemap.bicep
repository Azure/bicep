targetScope = 'tenant'

var managementGroups = [
//@    "managementGroups": [
//@    ]
  {
//@      {
//@      },
    name: 'one'
//@        "name": "one",
    displayName: 'The first'
//@        "displayName": "The first"
  }
  {
//@      {
//@      }
    name: 'two'
//@        "name": "two",
    displayName: 'The second'
//@        "displayName": "The second"
  }
]

resource singleGroup 'Microsoft.Management/managementGroups@2020-05-01' = {
//@    {
//@      "type": "Microsoft.Management/managementGroups",
//@      "apiVersion": "2020-05-01",
//@      "name": "myMG",
//@    },
  name: 'myMG'
  properties: {
//@      "properties": {
//@      }
    displayName: 'This one is mine!'
//@        "displayName": "This one is mine!"
  }
}

resource manyGroups 'Microsoft.Management/managementGroups@2020-05-01' = [for mg in managementGroups: {
//@    {
//@      "copy": {
//@        "name": "manyGroups",
//@        "count": "[length(variables('managementGroups'))]"
//@      },
//@      "type": "Microsoft.Management/managementGroups",
//@      "apiVersion": "2020-05-01",
//@      "name": "[variables('managementGroups')[copyIndex()].name]",
//@      "dependsOn": [
//@        "[tenantResourceId('Microsoft.Management/managementGroups', 'myMG')]"
//@      ]
//@    },
  name: mg.name
  properties: {
//@      "properties": {
//@      },
    displayName: '${mg.displayName} (${singleGroup.properties.displayName})'
//@        "displayName": "[format('{0} ({1})', variables('managementGroups')[copyIndex()].displayName, reference(tenantResourceId('Microsoft.Management/managementGroups', 'myMG'), '2020-05-01').displayName)]"
  }
}]

resource anotherSet 'Microsoft.Management/managementGroups@2020-05-01' = [for (mg, index) in managementGroups: {
//@    {
//@      "copy": {
//@        "name": "anotherSet",
//@        "count": "[length(variables('managementGroups'))]"
//@      },
//@      "type": "Microsoft.Management/managementGroups",
//@      "apiVersion": "2020-05-01",
//@      "name": "[concat(variables('managementGroups')[copyIndex()].name, '-one-', copyIndex())]",
//@      "dependsOn": [
//@        "manyGroups",
//@        "[tenantResourceId('Microsoft.Management/managementGroups', 'myMG')]"
//@      ]
//@    },
  name: concat(mg.name, '-one-', index)
  properties: {
//@      "properties": {
//@      },
    displayName: '${mg.displayName} (${singleGroup.properties.displayName}) (set 1) (index ${index})'
//@        "displayName": "[format('{0} ({1}) (set 1) (index {2})', variables('managementGroups')[copyIndex()].displayName, reference(tenantResourceId('Microsoft.Management/managementGroups', 'myMG'), '2020-05-01').displayName, copyIndex())]"
  }
  dependsOn: [
    manyGroups
  ]
}]

resource yetAnotherSet 'Microsoft.Management/managementGroups@2020-05-01' = [for mg in managementGroups: {
//@    {
//@      "copy": {
//@        "name": "yetAnotherSet",
//@        "count": "[length(variables('managementGroups'))]"
//@      },
//@      "type": "Microsoft.Management/managementGroups",
//@      "apiVersion": "2020-05-01",
//@      "name": "[concat(variables('managementGroups')[copyIndex()].name, '-two')]",
//@      "dependsOn": [
//@        "[tenantResourceId('Microsoft.Management/managementGroups', concat(variables('managementGroups')[0].name, '-one-', 0))]",
//@        "[tenantResourceId('Microsoft.Management/managementGroups', 'myMG')]"
//@      ]
//@    }
  name: concat(mg.name, '-two')
  properties: {
//@      "properties": {
//@      },
    displayName: '${mg.displayName} (${singleGroup.properties.displayName}) (set 2)'
//@        "displayName": "[format('{0} ({1}) (set 2)', variables('managementGroups')[copyIndex()].displayName, reference(tenantResourceId('Microsoft.Management/managementGroups', 'myMG'), '2020-05-01').displayName)]"
  }
  dependsOn: [
    anotherSet[0]
  ]
}]

output managementGroupIds array = [for i in range(0, length(managementGroups)): {
//@    "managementGroupIds": {
//@      "type": "array",
//@      "copy": {
//@        "count": "[length(range(0, length(variables('managementGroups'))))]",
//@        "input": {
//@        }
//@      }
//@    }
  name: yetAnotherSet[i].name
//@          "name": "[concat(variables('managementGroups')[range(0, length(variables('managementGroups')))[copyIndex()]].name, '-two')]",
  displayName: yetAnotherSet[i].properties.displayName
//@          "displayName": "[reference(tenantResourceId('Microsoft.Management/managementGroups', concat(variables('managementGroups')[range(0, length(variables('managementGroups')))[copyIndex()]].name, '-two')), '2020-05-01').displayName]"
}]

