{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "4287109754528407984"
    }
  },
  "parameters": {
    "createParent": {
      "type": "bool"
    },
    "createChild": {
      "type": "bool"
    },
    "createGrandchild": {
      "type": "bool"
    }
  },
  "variables": {
    "items": [
      "a",
      "b"
    ]
  },
  "resources": [
    {
      "type": "My.Rp/parentType/childType/grandchildType",
      "apiVersion": "2020-12-01",
      "name": "[format('{0}/{1}/{2}', 'basicParent', 'basicChild', 'basicGrandchild')]",
      "properties": {
        "size": "[reference(resourceId('My.Rp/parentType', 'basicParent'), '2020-12-01').size]",
        "style": "[reference(resourceId('My.Rp/parentType/childType', 'basicParent', 'basicChild'), '2020-12-01').style]"
      },
      "dependsOn": [
        "[resourceId('My.Rp/parentType', 'basicParent')]",
        "[resourceId('My.Rp/parentType/childType', 'basicParent', 'basicChild')]"
      ]
    },
    {
      "type": "My.Rp/parentType/childType",
      "apiVersion": "2020-12-01",
      "name": "[format('{0}/{1}', 'basicParent', 'basicChild')]",
      "properties": {
        "size": "[reference(resourceId('My.Rp/parentType', 'basicParent'), '2020-12-01').large]",
        "style": "cool"
      },
      "dependsOn": [
        "[resourceId('My.Rp/parentType', 'basicParent')]"
      ]
    },
    {
      "type": "My.Rp/parentType/childType",
      "apiVersion": "2020-12-01",
      "name": "[format('{0}/{1}', 'basicParent', 'basicSibling')]",
      "properties": {
        "size": "[reference(resourceId('My.Rp/parentType', 'basicParent'), '2020-12-01').size]",
        "style": "[reference(resourceId('My.Rp/parentType/childType/grandchildType', 'basicParent', 'basicChild', 'basicGrandchild'), '2020-12-01').style]"
      },
      "dependsOn": [
        "[resourceId('My.Rp/parentType', 'basicParent')]",
        "[resourceId('My.Rp/parentType/childType/grandchildType', 'basicParent', 'basicChild', 'basicGrandchild')]"
      ]
    },
    {
      "type": "My.Rp/parentType/childType/grandchildType",
      "apiVersion": "2020-12-01",
      "name": "[format('{0}/{1}/{2}', 'existingParent', 'existingChild', 'existingGrandchild')]",
      "properties": {
        "size": "[reference(resourceId('My.Rp/parentType', 'existingParent'), '2020-12-01').size]",
        "style": "[reference(resourceId('My.Rp/parentType/childType', 'existingParent', 'existingChild'), '2020-12-01').style]"
      }
    },
    {
      "condition": "[and(and(parameters('createParent'), parameters('createChild')), parameters('createGrandchild'))]",
      "type": "My.Rp/parentType/childType/grandchildType",
      "apiVersion": "2020-12-01",
      "name": "[format('{0}/{1}/{2}', 'conditionParent', 'conditionChild', 'conditionGrandchild')]",
      "properties": {
        "size": "[reference(resourceId('My.Rp/parentType', 'conditionParent'), '2020-12-01').size]",
        "style": "[reference(resourceId('My.Rp/parentType/childType', 'conditionParent', 'conditionChild'), '2020-12-01').style]"
      },
      "dependsOn": [
        "[resourceId('My.Rp/parentType', 'conditionParent')]",
        "[resourceId('My.Rp/parentType/childType', 'conditionParent', 'conditionChild')]"
      ]
    },
    {
      "condition": "[and(parameters('createParent'), parameters('createChild'))]",
      "type": "My.Rp/parentType/childType",
      "apiVersion": "2020-12-01",
      "name": "[format('{0}/{1}', 'conditionParent', 'conditionChild')]",
      "dependsOn": [
        "[resourceId('My.Rp/parentType', 'conditionParent')]"
      ]
    },
    {
      "copy": {
        "name": "loopParent::loopChild",
        "count": "[length(variables('items'))]"
      },
      "type": "My.Rp/parentType/childType",
      "apiVersion": "2020-12-01",
      "name": "[format('{0}/{1}', 'loopParent', 'loopChild')]",
      "dependsOn": [
        "[resourceId('My.Rp/parentType', 'loopParent')]"
      ]
    },
    {
      "type": "My.Rp/parentType",
      "apiVersion": "2020-12-01",
      "name": "basicParent",
      "properties": {
        "size": "large"
      }
    },
    {
      "condition": "[parameters('createParent')]",
      "type": "My.Rp/parentType",
      "apiVersion": "2020-12-01",
      "name": "conditionParent"
    },
    {
      "type": "My.Rp/parentType",
      "apiVersion": "2020-12-01",
      "name": "loopParent"
    }
  ],
  "outputs": {
    "referenceBasicChild": {
      "type": "string",
      "value": "[reference(resourceId('My.Rp/parentType/childType', 'basicParent', 'basicChild'), '2020-12-01').size]"
    },
    "referenceBasicGrandchild": {
      "type": "string",
      "value": "[reference(resourceId('My.Rp/parentType/childType/grandchildType', 'basicParent', 'basicChild', 'basicGrandchild'), '2020-12-01').style]"
    },
    "loopChildOutput": {
      "type": "string",
      "value": "loopChild"
    }
  }
}