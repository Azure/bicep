resource basicParent 'My.Rp/parentType@2020-12-01' = {
//@    {
//@      "type": "My.Rp/parentType",
//@      "apiVersion": "2020-12-01",
//@      "name": "basicParent",
//@    },
  name: 'basicParent'
  properties: {
//@      "properties": {
//@      }
    size: 'large'
//@        "size": "large"
  }

  resource basicChild 'childType' = {
//@    {
//@      "type": "My.Rp/parentType/childType",
//@      "apiVersion": "2020-12-01",
//@      "name": "[format('{0}/{1}', 'basicParent', 'basicChild')]",
//@      "dependsOn": [
//@        "[resourceId('My.Rp/parentType', 'basicParent')]"
//@      ]
//@    },
    name: 'basicChild'
    properties: {
//@      "properties": {
//@      },
      size: basicParent.properties.large
//@        "size": "[reference(resourceId('My.Rp/parentType', 'basicParent'), '2020-12-01').large]",
      style: 'cool'
//@        "style": "cool"
    }

    resource basicGrandchild 'grandchildType' = {
//@    {
//@      "type": "My.Rp/parentType/childType/grandchildType",
//@      "apiVersion": "2020-12-01",
//@      "name": "[format('{0}/{1}/{2}', 'basicParent', 'basicChild', 'basicGrandchild')]",
//@      "dependsOn": [
//@        "[resourceId('My.Rp/parentType', 'basicParent')]",
//@        "[resourceId('My.Rp/parentType/childType', 'basicParent', 'basicChild')]"
//@      ]
//@    },
      name: 'basicGrandchild'
      properties: {
//@      "properties": {
//@      },
        size: basicParent.properties.size
//@        "size": "[reference(resourceId('My.Rp/parentType', 'basicParent'), '2020-12-01').size]",
        style: basicChild.properties.style
//@        "style": "[reference(resourceId('My.Rp/parentType/childType', 'basicParent', 'basicChild'), '2020-12-01').style]"
      }
    }
  }

  resource basicSibling 'childType' = {
//@    {
//@      "type": "My.Rp/parentType/childType",
//@      "apiVersion": "2020-12-01",
//@      "name": "[format('{0}/{1}', 'basicParent', 'basicSibling')]",
//@      "dependsOn": [
//@        "[resourceId('My.Rp/parentType', 'basicParent')]",
//@        "[resourceId('My.Rp/parentType/childType/grandchildType', 'basicParent', 'basicChild', 'basicGrandchild')]"
//@      ]
//@    },
    name: 'basicSibling'
    properties: {
//@      "properties": {
//@      },
      size: basicParent.properties.size
//@        "size": "[reference(resourceId('My.Rp/parentType', 'basicParent'), '2020-12-01').size]",
      style: basicChild::basicGrandchild.properties.style
//@        "style": "[reference(resourceId('My.Rp/parentType/childType/grandchildType', 'basicParent', 'basicChild', 'basicGrandchild'), '2020-12-01').style]"
    }
  }
}
// #completionTest(50) -> childResources
output referenceBasicChild string = basicParent::basicChild.properties.size
//@    "referenceBasicChild": {
//@      "type": "string",
//@      "value": "[reference(resourceId('My.Rp/parentType/childType', 'basicParent', 'basicChild'), '2020-12-01').size]"
//@    },
// #completionTest(67) -> grandChildResources
output referenceBasicGrandchild string = basicParent::basicChild::basicGrandchild.properties.style
//@    "referenceBasicGrandchild": {
//@      "type": "string",
//@      "value": "[reference(resourceId('My.Rp/parentType/childType/grandchildType', 'basicParent', 'basicChild', 'basicGrandchild'), '2020-12-01').style]"
//@    },

resource existingParent 'My.Rp/parentType@2020-12-01' existing = {
  name: 'existingParent'

  resource existingChild 'childType' existing = {
    name: 'existingChild'

    resource existingGrandchild 'grandchildType' = {
//@    {
//@      "type": "My.Rp/parentType/childType/grandchildType",
//@      "apiVersion": "2020-12-01",
//@      "name": "[format('{0}/{1}/{2}', 'existingParent', 'existingChild', 'existingGrandchild')]",
//@    },
      name: 'existingGrandchild'
      properties: {
//@      "properties": {
//@      }
        size: existingParent.properties.size
//@        "size": "[reference(resourceId('My.Rp/parentType', 'existingParent'), '2020-12-01').size]",
        style: existingChild.properties.style
//@        "style": "[reference(resourceId('My.Rp/parentType/childType', 'existingParent', 'existingChild'), '2020-12-01').style]"
      }
    }
  }
}

param createParent bool
//@    "createParent": {
//@      "type": "bool"
//@    },
param createChild bool
//@    "createChild": {
//@      "type": "bool"
//@    },
param createGrandchild bool
//@    "createGrandchild": {
//@      "type": "bool"
//@    }
resource conditionParent 'My.Rp/parentType@2020-12-01' = if (createParent) {
//@    {
//@      "condition": "[parameters('createParent')]",
//@      "type": "My.Rp/parentType",
//@      "apiVersion": "2020-12-01",
//@      "name": "conditionParent"
//@    },
  name: 'conditionParent'

  resource conditionChild 'childType' = if (createChild) {
//@    {
//@      "condition": "[and(parameters('createParent'), parameters('createChild'))]",
//@      "type": "My.Rp/parentType/childType",
//@      "apiVersion": "2020-12-01",
//@      "name": "[format('{0}/{1}', 'conditionParent', 'conditionChild')]",
//@      "dependsOn": [
//@        "[resourceId('My.Rp/parentType', 'conditionParent')]"
//@      ]
//@    },
    name: 'conditionChild'

    resource conditionGrandchild 'grandchildType' = if (createGrandchild) {
//@    {
//@      "condition": "[and(and(parameters('createParent'), parameters('createChild')), parameters('createGrandchild'))]",
//@      "type": "My.Rp/parentType/childType/grandchildType",
//@      "apiVersion": "2020-12-01",
//@      "name": "[format('{0}/{1}/{2}', 'conditionParent', 'conditionChild', 'conditionGrandchild')]",
//@      "dependsOn": [
//@        "[resourceId('My.Rp/parentType', 'conditionParent')]",
//@        "[resourceId('My.Rp/parentType/childType', 'conditionParent', 'conditionChild')]"
//@      ]
//@    },
      name: 'conditionGrandchild'
      properties: {
//@      "properties": {
//@      },
        size: conditionParent.properties.size
//@        "size": "[reference(resourceId('My.Rp/parentType', 'conditionParent'), '2020-12-01').size]",
        style: conditionChild.properties.style
//@        "style": "[reference(resourceId('My.Rp/parentType/childType', 'conditionParent', 'conditionChild'), '2020-12-01').style]"
      }
    }
  }
}

var items = [
//@    "items": [
//@    ]
  'a'
//@      "a",
  'b'
//@      "b"
]
resource loopParent 'My.Rp/parentType@2020-12-01' = {
//@    {
//@      "type": "My.Rp/parentType",
//@      "apiVersion": "2020-12-01",
//@      "name": "loopParent"
//@    }
  name: 'loopParent'

  resource loopChild 'childType' = [for item in items: {
//@    {
//@      "copy": {
//@        "name": "loopParent::loopChild",
//@        "count": "[length(variables('items'))]"
//@      },
//@      "type": "My.Rp/parentType/childType",
//@      "apiVersion": "2020-12-01",
//@      "name": "[format('{0}/{1}', 'loopParent', 'loopChild')]",
//@      "dependsOn": [
//@        "[resourceId('My.Rp/parentType', 'loopParent')]"
//@      ]
//@    },
    name: 'loopChild'
  }]
}

output loopChildOutput string = loopParent::loopChild[0].name
//@    "loopChildOutput": {
//@      "type": "string",
//@      "value": "loopChild"
//@    }
