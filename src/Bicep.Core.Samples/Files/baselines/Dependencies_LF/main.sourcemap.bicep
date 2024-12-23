param deployTimeParam string = 'steve'
//@    "deployTimeParam": {
//@      "type": "string",
//@      "defaultValue": "steve"
//@    }
var deployTimeVar = 'nigel'
//@    "deployTimeVar": "nigel",
var dependentVar = {
//@    "dependentVar": {
//@    },
  dependencies: [
//@      "dependencies": [
//@      ]
    deployTimeVar
//@        "[variables('deployTimeVar')]",
    deployTimeParam
//@        "[parameters('deployTimeParam')]"
  ]
}

var resourceDependency = {
//@        "dependencies": {
//@        }
  dependenciesA: [
//@          "dependenciesA": [
//@          ]
    resA.id
//@            "[resourceId('My.Rp/myResourceType', 'resA')]",
    resA.name
//@            "resA",
    resA.type
//@            "My.Rp/myResourceType",
    resA.properties.deployTime
//@            "[reference(resourceId('My.Rp/myResourceType', 'resA'), '2020-01-01').deployTime]",
    resA.properties.eTag
//@            "[reference(resourceId('My.Rp/myResourceType', 'resA'), '2020-01-01').eTag]"
  ]
}

output resourceAType string = resA.type
//@    "resourceAType": {
//@      "type": "string",
//@      "value": "My.Rp/myResourceType"
//@    },
resource resA 'My.Rp/myResourceType@2020-01-01' = {
//@    {
//@      "type": "My.Rp/myResourceType",
//@      "apiVersion": "2020-01-01",
//@      "name": "resA",
//@    },
  name: 'resA'
  properties: {
//@      "properties": {
//@      }
    deployTime: dependentVar
//@        "deployTime": "[variables('dependentVar')]",
    eTag: '1234'
//@        "eTag": "1234"
  }
}

output resourceBId string = resB.id
//@    "resourceBId": {
//@      "type": "string",
//@      "value": "[resourceId('My.Rp/myResourceType', 'resB')]"
//@    },
resource resB 'My.Rp/myResourceType@2020-01-01' = {
//@    {
//@      "type": "My.Rp/myResourceType",
//@      "apiVersion": "2020-01-01",
//@      "name": "resB",
//@      "dependsOn": [
//@        "[resourceId('My.Rp/myResourceType', 'resA')]"
//@      ]
//@    },
  name: 'resB'
  properties: {
//@      "properties": {
//@      },
    dependencies: resourceDependency
  }
}

var resourceIds = {
//@    "resourceIds": {
//@    }
  a: resA.id
//@      "a": "[resourceId('My.Rp/myResourceType', 'resA')]",
  b: resB.id
//@      "b": "[resourceId('My.Rp/myResourceType', 'resB')]"
}

resource resC 'My.Rp/myResourceType@2020-01-01' = {
//@    {
//@      "type": "My.Rp/myResourceType",
//@      "apiVersion": "2020-01-01",
//@      "name": "resC",
//@      "dependsOn": [
//@        "[resourceId('My.Rp/myResourceType', 'resA')]",
//@        "[resourceId('My.Rp/myResourceType', 'resB')]"
//@      ]
//@    },
  name: 'resC'
  properties: {
//@      "properties": {
//@      },
    resourceIds: resourceIds
//@        "resourceIds": "[variables('resourceIds')]"
  }
}

resource resD 'My.Rp/myResourceType/childType@2020-01-01' = {
//@    {
//@      "type": "My.Rp/myResourceType/childType",
//@      "apiVersion": "2020-01-01",
//@      "name": "[format('{0}/resD', 'resC')]",
//@      "dependsOn": [
//@        "[resourceId('My.Rp/myResourceType', 'resC')]"
//@      ]
//@    },
  name: '${resC.name}/resD'
  properties: {
//@      "properties": {},
  }
}

resource resE 'My.Rp/myResourceType/childType@2020-01-01' = {
//@    {
//@      "type": "My.Rp/myResourceType/childType",
//@      "apiVersion": "2020-01-01",
//@      "name": "resC/resD_2",
//@      "dependsOn": [
//@        "[resourceId('My.Rp/myResourceType/childType', split(format('{0}/resD', 'resC'), '/')[0], split(format('{0}/resD', 'resC'), '/')[1])]"
//@      ]
//@    }
  name: 'resC/resD_2'
  properties: {
//@      "properties": {
//@      },
    resDRef: resD.id
//@        "resDRef": "[resourceId('My.Rp/myResourceType/childType', split(format('{0}/resD', 'resC'), '/')[0], split(format('{0}/resD', 'resC'), '/')[1])]"
  }
}

output resourceCProperties object = resC.properties
//@    "resourceCProperties": {
//@      "type": "object",
//@      "value": "[reference(resourceId('My.Rp/myResourceType', 'resC'), '2020-01-01')]"
//@    }

