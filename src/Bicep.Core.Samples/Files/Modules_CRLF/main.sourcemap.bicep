
@sys.description('this is deployTimeSuffix param')
//@        "description": "this is deployTimeSuffix param"
param deployTimeSuffix string = newGuid()
//@    "deployTimeSuffix": {
//@      "type": "string",
//@      "defaultValue": "[newGuid()]",
//@      "metadata": {
//@      }
//@    }

@sys.description('this module a')
//@        "description": "this module a"
module modATest './modulea.bicep' = {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2020-10-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "parameters": {
//@          "stringParamB": {
//@          },
//@          "objParam": {
//@          },
//@          "arrayParam": {
//@          }
//@        },
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "8300391961099598421"
//@            }
//@          },
//@          "parameters": {
//@            "stringParamA": {
//@              "type": "string",
//@              "defaultValue": "test"
//@            },
//@            "stringParamB": {
//@              "type": "string"
//@            },
//@            "objParam": {
//@              "type": "object"
//@            },
//@            "arrayParam": {
//@              "type": "array"
//@            }
//@          },
//@          "resources": [
//@            {
//@              "type": "Mock.Rp/mockResource",
//@              "apiVersion": "2020-01-01",
//@              "name": "basicblobs",
//@              "location": "[parameters('stringParamA')]"
//@            },
//@            {
//@              "type": "Mock.Rp/mockResource",
//@              "apiVersion": "2020-01-01",
//@              "name": "myZone",
//@              "location": "[parameters('stringParamB')]"
//@            }
//@          ],
//@          "outputs": {
//@            "stringOutputA": {
//@              "type": "string",
//@              "value": "[parameters('stringParamA')]"
//@            },
//@            "stringOutputB": {
//@              "type": "string",
//@              "value": "[parameters('stringParamB')]"
//@            },
//@            "objOutput": {
//@              "type": "object",
//@              "value": "[reference(resourceId('Mock.Rp/mockResource', 'basicblobs'), '2020-01-01')]"
//@            },
//@            "arrayOutput": {
//@              "type": "array",
//@              "value": [
//@                "[resourceId('Mock.Rp/mockResource', 'basicblobs')]",
//@                "[resourceId('Mock.Rp/mockResource', 'myZone')]"
//@              ]
//@            }
//@          }
//@        }
//@      },
//@      "metadata": {
//@      }
//@    },
  name: 'modATest'
//@      "name": "modATest",
  params: {
    stringParamB: 'hello!'
//@            "value": "hello!"
    objParam: {
//@            "value": {
//@            }
      a: 'b'
//@              "a": "b"
    }
    arrayParam: [
//@            "value": [
//@            ]
      {
//@              {
//@              },
        a: 'b'
//@                "a": "b"
      }
      'abc'
//@              "abc"
    ]
  }
}


@sys.description('this module b')
//@        "description": "this module b"
module modB './child/moduleb.bicep' = {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2020-10-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "parameters": {
//@          "location": {
//@          }
//@        },
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "13693869390953445824"
//@            }
//@          },
//@          "parameters": {
//@            "location": {
//@              "type": "string"
//@            }
//@          },
//@          "resources": [
//@            {
//@              "type": "Mock.Rp/mockResource",
//@              "apiVersion": "2020-01-01",
//@              "name": "mockResource",
//@              "location": "[parameters('location')]"
//@            }
//@          ],
//@          "outputs": {
//@            "myResourceId": {
//@              "type": "string",
//@              "value": "[resourceId('Mock.Rp/mockResource', 'mockResource')]"
//@            }
//@          }
//@        }
//@      },
//@      "metadata": {
//@      }
//@    },
  name: 'modB'
//@      "name": "modB",
  params: {
    location: 'West US'
//@            "value": "West US"
  }
}

@sys.description('this is just module b with a condition')
//@        "description": "this is just module b with a condition"
module modBWithCondition './child/moduleb.bicep' = if (1 + 1 == 2) {
//@    {
//@      "condition": "[equals(add(1, 1), 2)]",
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2020-10-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "parameters": {
//@          "location": {
//@          }
//@        },
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "13693869390953445824"
//@            }
//@          },
//@          "parameters": {
//@            "location": {
//@              "type": "string"
//@            }
//@          },
//@          "resources": [
//@            {
//@              "type": "Mock.Rp/mockResource",
//@              "apiVersion": "2020-01-01",
//@              "name": "mockResource",
//@              "location": "[parameters('location')]"
//@            }
//@          ],
//@          "outputs": {
//@            "myResourceId": {
//@              "type": "string",
//@              "value": "[resourceId('Mock.Rp/mockResource', 'mockResource')]"
//@            }
//@          }
//@        }
//@      },
//@      "metadata": {
//@      }
//@    },
  name: 'modBWithCondition'
//@      "name": "modBWithCondition",
  params: {
    location: 'East US'
//@            "value": "East US"
  }
}

module modC './child/modulec.json' = {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2020-10-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "parameters": {
//@          "location": {
//@          }
//@        },
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "parameters": {
//@            "location": {
//@              "type": "string"
//@            }
//@          },
//@          "variables": {},
//@          "resources": [
//@            {
//@              "name": "myResource",
//@              "type": "Mock.Rp/mockResource",
//@              "apiVersion": "2020-01-01",
//@              "location": "[parameters('location')]"
//@            }
//@          ],
//@          "outputs": {
//@            "myResourceId": {
//@              "type": "string",
//@              "value": "[resourceId('Mock.Rp/mockResource', 'myResource')]"
//@            }
//@          }
//@        }
//@      }
//@    },
  name: 'modC'
//@      "name": "modC",
  params: {
    location: 'West US'
//@            "value": "West US"
  }
}

module modCWithCondition './child/modulec.json' = if (2 - 1 == 1) {
//@    {
//@      "condition": "[equals(sub(2, 1), 1)]",
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2020-10-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "parameters": {
//@          "location": {
//@          }
//@        },
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "parameters": {
//@            "location": {
//@              "type": "string"
//@            }
//@          },
//@          "variables": {},
//@          "resources": [
//@            {
//@              "name": "myResource",
//@              "type": "Mock.Rp/mockResource",
//@              "apiVersion": "2020-01-01",
//@              "location": "[parameters('location')]"
//@            }
//@          ],
//@          "outputs": {
//@            "myResourceId": {
//@              "type": "string",
//@              "value": "[resourceId('Mock.Rp/mockResource', 'myResource')]"
//@            }
//@          }
//@        }
//@      }
//@    },
  name: 'modCWithCondition'
//@      "name": "modCWithCondition",
  params: {
    location: 'East US'
//@            "value": "East US"
  }
}

module optionalWithNoParams1 './child/optionalParams.bicep'= {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2020-10-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "4191259681487754679"
//@            }
//@          },
//@          "parameters": {
//@            "optionalString": {
//@              "type": "string",
//@              "defaultValue": "abc"
//@            },
//@            "optionalInt": {
//@              "type": "int",
//@              "defaultValue": 42
//@            },
//@            "optionalObj": {
//@              "type": "object",
//@              "defaultValue": {
//@                "a": "b"
//@              }
//@            },
//@            "optionalArray": {
//@              "type": "array",
//@              "defaultValue": [
//@                1,
//@                2,
//@                3
//@              ]
//@            }
//@          },
//@          "resources": [],
//@          "outputs": {
//@            "outputObj": {
//@              "type": "object",
//@              "value": {
//@                "optionalString": "[parameters('optionalString')]",
//@                "optionalInt": "[parameters('optionalInt')]",
//@                "optionalObj": "[parameters('optionalObj')]",
//@                "optionalArray": "[parameters('optionalArray')]"
//@              }
//@            }
//@          }
//@        }
//@      }
//@    },
  name: 'optionalWithNoParams1'
//@      "name": "optionalWithNoParams1",
}

module optionalWithNoParams2 './child/optionalParams.bicep'= {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2020-10-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "parameters": {},
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "4191259681487754679"
//@            }
//@          },
//@          "parameters": {
//@            "optionalString": {
//@              "type": "string",
//@              "defaultValue": "abc"
//@            },
//@            "optionalInt": {
//@              "type": "int",
//@              "defaultValue": 42
//@            },
//@            "optionalObj": {
//@              "type": "object",
//@              "defaultValue": {
//@                "a": "b"
//@              }
//@            },
//@            "optionalArray": {
//@              "type": "array",
//@              "defaultValue": [
//@                1,
//@                2,
//@                3
//@              ]
//@            }
//@          },
//@          "resources": [],
//@          "outputs": {
//@            "outputObj": {
//@              "type": "object",
//@              "value": {
//@                "optionalString": "[parameters('optionalString')]",
//@                "optionalInt": "[parameters('optionalInt')]",
//@                "optionalObj": "[parameters('optionalObj')]",
//@                "optionalArray": "[parameters('optionalArray')]"
//@              }
//@            }
//@          }
//@        }
//@      }
//@    },
  name: 'optionalWithNoParams2'
//@      "name": "optionalWithNoParams2",
  params: {
  }
}

module optionalWithAllParams './child/optionalParams.bicep'= {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2020-10-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "parameters": {
//@          "optionalString": {
//@          },
//@          "optionalInt": {
//@          },
//@          "optionalObj": {
//@          },
//@          "optionalArray": {
//@          }
//@        },
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "4191259681487754679"
//@            }
//@          },
//@          "parameters": {
//@            "optionalString": {
//@              "type": "string",
//@              "defaultValue": "abc"
//@            },
//@            "optionalInt": {
//@              "type": "int",
//@              "defaultValue": 42
//@            },
//@            "optionalObj": {
//@              "type": "object",
//@              "defaultValue": {
//@                "a": "b"
//@              }
//@            },
//@            "optionalArray": {
//@              "type": "array",
//@              "defaultValue": [
//@                1,
//@                2,
//@                3
//@              ]
//@            }
//@          },
//@          "resources": [],
//@          "outputs": {
//@            "outputObj": {
//@              "type": "object",
//@              "value": {
//@                "optionalString": "[parameters('optionalString')]",
//@                "optionalInt": "[parameters('optionalInt')]",
//@                "optionalObj": "[parameters('optionalObj')]",
//@                "optionalArray": "[parameters('optionalArray')]"
//@              }
//@            }
//@          }
//@        }
//@      }
//@    },
  name: 'optionalWithNoParams3'
//@      "name": "optionalWithNoParams3",
  params: {
    optionalString: 'abc'
//@            "value": "abc"
    optionalInt: 42
//@            "value": 42
    optionalObj: { }
//@            "value": {}
    optionalArray: [ ]
//@            "value": []
  }
}

resource resWithDependencies 'Mock.Rp/mockResource@2020-01-01' = {
//@    {
//@      "type": "Mock.Rp/mockResource",
//@      "apiVersion": "2020-01-01",
//@      "name": "harry",
//@      "dependsOn": [
//@        "[resourceId('Microsoft.Resources/deployments', 'modATest')]",
//@        "[resourceId('Microsoft.Resources/deployments', 'modB')]",
//@        "[resourceId('Microsoft.Resources/deployments', 'modC')]"
//@      ]
//@    },
  name: 'harry'
  properties: {
//@      "properties": {
//@      },
    modADep: modATest.outputs.stringOutputA
//@        "modADep": "[reference(resourceId('Microsoft.Resources/deployments', 'modATest'), '2020-10-01').outputs.stringOutputA.value]",
    modBDep: modB.outputs.myResourceId
//@        "modBDep": "[reference(resourceId('Microsoft.Resources/deployments', 'modB'), '2020-10-01').outputs.myResourceId.value]",
    modCDep: modC.outputs.myResourceId
//@        "modCDep": "[reference(resourceId('Microsoft.Resources/deployments', 'modC'), '2020-10-01').outputs.myResourceId.value]"
  }
}

module optionalWithAllParamsAndManualDependency './child/optionalParams.bicep'= {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2020-10-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "parameters": {
//@          "optionalString": {
//@          },
//@          "optionalInt": {
//@          },
//@          "optionalObj": {
//@          },
//@          "optionalArray": {
//@          }
//@        },
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "4191259681487754679"
//@            }
//@          },
//@          "parameters": {
//@            "optionalString": {
//@              "type": "string",
//@              "defaultValue": "abc"
//@            },
//@            "optionalInt": {
//@              "type": "int",
//@              "defaultValue": 42
//@            },
//@            "optionalObj": {
//@              "type": "object",
//@              "defaultValue": {
//@                "a": "b"
//@              }
//@            },
//@            "optionalArray": {
//@              "type": "array",
//@              "defaultValue": [
//@                1,
//@                2,
//@                3
//@              ]
//@            }
//@          },
//@          "resources": [],
//@          "outputs": {
//@            "outputObj": {
//@              "type": "object",
//@              "value": {
//@                "optionalString": "[parameters('optionalString')]",
//@                "optionalInt": "[parameters('optionalInt')]",
//@                "optionalObj": "[parameters('optionalObj')]",
//@                "optionalArray": "[parameters('optionalArray')]"
//@              }
//@            }
//@          }
//@        }
//@      },
//@      "dependsOn": [
//@        "[resourceId('Microsoft.Resources/deployments', 'optionalWithNoParams3')]",
//@        "[resourceId('Mock.Rp/mockResource', 'harry')]"
//@      ]
//@    },
  name: 'optionalWithAllParamsAndManualDependency'
//@      "name": "optionalWithAllParamsAndManualDependency",
  params: {
    optionalString: 'abc'
//@            "value": "abc"
    optionalInt: 42
//@            "value": 42
    optionalObj: { }
//@            "value": {}
    optionalArray: [ ]
//@            "value": []
  }
  dependsOn: [
    resWithDependencies
    optionalWithAllParams
  ]
}

module optionalWithImplicitDependency './child/optionalParams.bicep'= {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2020-10-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "parameters": {
//@          "optionalString": {
//@          },
//@          "optionalInt": {
//@          },
//@          "optionalObj": {
//@          },
//@          "optionalArray": {
//@          }
//@        },
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "4191259681487754679"
//@            }
//@          },
//@          "parameters": {
//@            "optionalString": {
//@              "type": "string",
//@              "defaultValue": "abc"
//@            },
//@            "optionalInt": {
//@              "type": "int",
//@              "defaultValue": 42
//@            },
//@            "optionalObj": {
//@              "type": "object",
//@              "defaultValue": {
//@                "a": "b"
//@              }
//@            },
//@            "optionalArray": {
//@              "type": "array",
//@              "defaultValue": [
//@                1,
//@                2,
//@                3
//@              ]
//@            }
//@          },
//@          "resources": [],
//@          "outputs": {
//@            "outputObj": {
//@              "type": "object",
//@              "value": {
//@                "optionalString": "[parameters('optionalString')]",
//@                "optionalInt": "[parameters('optionalInt')]",
//@                "optionalObj": "[parameters('optionalObj')]",
//@                "optionalArray": "[parameters('optionalArray')]"
//@              }
//@            }
//@          }
//@        }
//@      },
//@      "dependsOn": [
//@        "[resourceId('Microsoft.Resources/deployments', 'optionalWithAllParamsAndManualDependency')]",
//@        "[resourceId('Mock.Rp/mockResource', 'harry')]"
//@      ]
//@    },
  name: 'optionalWithImplicitDependency'
//@      "name": "optionalWithImplicitDependency",
  params: {
    optionalString: concat(resWithDependencies.id, optionalWithAllParamsAndManualDependency.name)
//@            "value": "[concat(resourceId('Mock.Rp/mockResource', 'harry'), 'optionalWithAllParamsAndManualDependency')]"
    optionalInt: 42
//@            "value": 42
    optionalObj: { }
//@            "value": {}
    optionalArray: [ ]
//@            "value": []
  }
}

module moduleWithCalculatedName './child/optionalParams.bicep'= {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2020-10-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "parameters": {
//@          "optionalString": {
//@          },
//@          "optionalInt": {
//@          },
//@          "optionalObj": {
//@          },
//@          "optionalArray": {
//@          }
//@        },
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "4191259681487754679"
//@            }
//@          },
//@          "parameters": {
//@            "optionalString": {
//@              "type": "string",
//@              "defaultValue": "abc"
//@            },
//@            "optionalInt": {
//@              "type": "int",
//@              "defaultValue": 42
//@            },
//@            "optionalObj": {
//@              "type": "object",
//@              "defaultValue": {
//@                "a": "b"
//@              }
//@            },
//@            "optionalArray": {
//@              "type": "array",
//@              "defaultValue": [
//@                1,
//@                2,
//@                3
//@              ]
//@            }
//@          },
//@          "resources": [],
//@          "outputs": {
//@            "outputObj": {
//@              "type": "object",
//@              "value": {
//@                "optionalString": "[parameters('optionalString')]",
//@                "optionalInt": "[parameters('optionalInt')]",
//@                "optionalObj": "[parameters('optionalObj')]",
//@                "optionalArray": "[parameters('optionalArray')]"
//@              }
//@            }
//@          }
//@        }
//@      },
//@      "dependsOn": [
//@        "[resourceId('Microsoft.Resources/deployments', 'optionalWithAllParamsAndManualDependency')]",
//@        "[resourceId('Mock.Rp/mockResource', 'harry')]"
//@      ]
//@    },
  name: '${optionalWithAllParamsAndManualDependency.name}${deployTimeSuffix}'
//@      "name": "[format('{0}{1}', 'optionalWithAllParamsAndManualDependency', parameters('deployTimeSuffix'))]",
  params: {
    optionalString: concat(resWithDependencies.id, optionalWithAllParamsAndManualDependency.name)
//@            "value": "[concat(resourceId('Mock.Rp/mockResource', 'harry'), 'optionalWithAllParamsAndManualDependency')]"
    optionalInt: 42
//@            "value": 42
    optionalObj: { }
//@            "value": {}
    optionalArray: [ ]
//@            "value": []
  }
}

resource resWithCalculatedNameDependencies 'Mock.Rp/mockResource@2020-01-01' = {
//@    {
//@      "type": "Mock.Rp/mockResource",
//@      "apiVersion": "2020-01-01",
//@      "name": "[format('{0}{1}', 'optionalWithAllParamsAndManualDependency', parameters('deployTimeSuffix'))]",
//@      "dependsOn": [
//@        "[resourceId('Microsoft.Resources/deployments', format('{0}{1}', 'optionalWithAllParamsAndManualDependency', parameters('deployTimeSuffix')))]",
//@        "[resourceId('Microsoft.Resources/deployments', 'optionalWithAllParamsAndManualDependency')]"
//@      ]
//@    },
  name: '${optionalWithAllParamsAndManualDependency.name}${deployTimeSuffix}'
  properties: {
//@      "properties": {
//@      },
    modADep: moduleWithCalculatedName.outputs.outputObj
//@        "modADep": "[reference(resourceId('Microsoft.Resources/deployments', format('{0}{1}', 'optionalWithAllParamsAndManualDependency', parameters('deployTimeSuffix'))), '2020-10-01').outputs.outputObj.value]"
  }
}

output stringOutputA string = modATest.outputs.stringOutputA
//@    "stringOutputA": {
//@      "type": "string",
//@      "value": "[reference(resourceId('Microsoft.Resources/deployments', 'modATest'), '2020-10-01').outputs.stringOutputA.value]"
//@    },
output stringOutputB string = modATest.outputs.stringOutputB
//@    "stringOutputB": {
//@      "type": "string",
//@      "value": "[reference(resourceId('Microsoft.Resources/deployments', 'modATest'), '2020-10-01').outputs.stringOutputB.value]"
//@    },
output objOutput object = modATest.outputs.objOutput
//@    "objOutput": {
//@      "type": "object",
//@      "value": "[reference(resourceId('Microsoft.Resources/deployments', 'modATest'), '2020-10-01').outputs.objOutput.value]"
//@    },
output arrayOutput array = modATest.outputs.arrayOutput
//@    "arrayOutput": {
//@      "type": "array",
//@      "value": "[reference(resourceId('Microsoft.Resources/deployments', 'modATest'), '2020-10-01').outputs.arrayOutput.value]"
//@    },
output modCalculatedNameOutput object = moduleWithCalculatedName.outputs.outputObj
//@    "modCalculatedNameOutput": {
//@      "type": "object",
//@      "value": "[reference(resourceId('Microsoft.Resources/deployments', format('{0}{1}', 'optionalWithAllParamsAndManualDependency', parameters('deployTimeSuffix'))), '2020-10-01').outputs.outputObj.value]"
//@    }

/*
  valid loop cases
*/

@sys.description('this is myModules')
var myModules = [
//@    "myModules": [
//@    ],
  {
//@      {
//@      },
    name: 'one'
//@        "name": "one",
    location: 'eastus2'
//@        "location": "eastus2"
  }
  {
//@      {
//@      }
    name: 'two'
//@        "name": "two",
    location: 'westus'
//@        "location": "westus"
  }
]

var emptyArray = []
//@    "emptyArray": [],

// simple module loop
module storageResources 'modulea.bicep' = [for module in myModules: {
//@    {
//@      "copy": {
//@        "name": "storageResources",
//@        "count": "[length(variables('myModules'))]"
//@      },
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2020-10-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "parameters": {
//@          "arrayParam": {
//@          },
//@          "objParam": {
//@          },
//@          "stringParamB": {
//@          }
//@        },
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "8300391961099598421"
//@            }
//@          },
//@          "parameters": {
//@            "stringParamA": {
//@              "type": "string",
//@              "defaultValue": "test"
//@            },
//@            "stringParamB": {
//@              "type": "string"
//@            },
//@            "objParam": {
//@              "type": "object"
//@            },
//@            "arrayParam": {
//@              "type": "array"
//@            }
//@          },
//@          "resources": [
//@            {
//@              "type": "Mock.Rp/mockResource",
//@              "apiVersion": "2020-01-01",
//@              "name": "basicblobs",
//@              "location": "[parameters('stringParamA')]"
//@            },
//@            {
//@              "type": "Mock.Rp/mockResource",
//@              "apiVersion": "2020-01-01",
//@              "name": "myZone",
//@              "location": "[parameters('stringParamB')]"
//@            }
//@          ],
//@          "outputs": {
//@            "stringOutputA": {
//@              "type": "string",
//@              "value": "[parameters('stringParamA')]"
//@            },
//@            "stringOutputB": {
//@              "type": "string",
//@              "value": "[parameters('stringParamB')]"
//@            },
//@            "objOutput": {
//@              "type": "object",
//@              "value": "[reference(resourceId('Mock.Rp/mockResource', 'basicblobs'), '2020-01-01')]"
//@            },
//@            "arrayOutput": {
//@              "type": "array",
//@              "value": [
//@                "[resourceId('Mock.Rp/mockResource', 'basicblobs')]",
//@                "[resourceId('Mock.Rp/mockResource', 'myZone')]"
//@              ]
//@            }
//@          }
//@        }
//@      }
//@    },
  name: module.name
//@      "name": "[variables('myModules')[copyIndex()].name]",
  params: {
    arrayParam: []
//@            "value": []
    objParam: module
//@            "value": "[variables('myModules')[copyIndex()]]"
    stringParamB: module.location
//@            "value": "[variables('myModules')[copyIndex()].location]"
  }
}]

// simple indexed module loop
module storageResourcesWithIndex 'modulea.bicep' = [for (module, i) in myModules: {
//@    {
//@      "copy": {
//@        "name": "storageResourcesWithIndex",
//@        "count": "[length(variables('myModules'))]"
//@      },
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2020-10-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "parameters": {
//@          "arrayParam": {
//@          },
//@          "objParam": {
//@          },
//@          "stringParamB": {
//@          },
//@          "stringParamA": {
//@          }
//@        },
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "8300391961099598421"
//@            }
//@          },
//@          "parameters": {
//@            "stringParamA": {
//@              "type": "string",
//@              "defaultValue": "test"
//@            },
//@            "stringParamB": {
//@              "type": "string"
//@            },
//@            "objParam": {
//@              "type": "object"
//@            },
//@            "arrayParam": {
//@              "type": "array"
//@            }
//@          },
//@          "resources": [
//@            {
//@              "type": "Mock.Rp/mockResource",
//@              "apiVersion": "2020-01-01",
//@              "name": "basicblobs",
//@              "location": "[parameters('stringParamA')]"
//@            },
//@            {
//@              "type": "Mock.Rp/mockResource",
//@              "apiVersion": "2020-01-01",
//@              "name": "myZone",
//@              "location": "[parameters('stringParamB')]"
//@            }
//@          ],
//@          "outputs": {
//@            "stringOutputA": {
//@              "type": "string",
//@              "value": "[parameters('stringParamA')]"
//@            },
//@            "stringOutputB": {
//@              "type": "string",
//@              "value": "[parameters('stringParamB')]"
//@            },
//@            "objOutput": {
//@              "type": "object",
//@              "value": "[reference(resourceId('Mock.Rp/mockResource', 'basicblobs'), '2020-01-01')]"
//@            },
//@            "arrayOutput": {
//@              "type": "array",
//@              "value": [
//@                "[resourceId('Mock.Rp/mockResource', 'basicblobs')]",
//@                "[resourceId('Mock.Rp/mockResource', 'myZone')]"
//@              ]
//@            }
//@          }
//@        }
//@      }
//@    },
  name: module.name
//@      "name": "[variables('myModules')[copyIndex()].name]",
  params: {
    arrayParam: [
//@            "value": [
//@            ]
      i + 1
//@              "[add(copyIndex(), 1)]"
    ]
    objParam: module
//@            "value": "[variables('myModules')[copyIndex()]]"
    stringParamB: module.location
//@            "value": "[variables('myModules')[copyIndex()].location]"
    stringParamA: concat('a', i)
//@            "value": "[concat('a', copyIndex())]"
  }
}]

// nested module loop
module nestedModuleLoop 'modulea.bicep' = [for module in myModules: {
//@    {
//@      "copy": {
//@        "name": "nestedModuleLoop",
//@        "count": "[length(variables('myModules'))]"
//@      },
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2020-10-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "parameters": {
//@          "arrayParam": {
//@            "copy": [
//@            ]
//@          },
//@          "objParam": {
//@          },
//@          "stringParamB": {
//@          }
//@        },
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "8300391961099598421"
//@            }
//@          },
//@          "parameters": {
//@            "stringParamA": {
//@              "type": "string",
//@              "defaultValue": "test"
//@            },
//@            "stringParamB": {
//@              "type": "string"
//@            },
//@            "objParam": {
//@              "type": "object"
//@            },
//@            "arrayParam": {
//@              "type": "array"
//@            }
//@          },
//@          "resources": [
//@            {
//@              "type": "Mock.Rp/mockResource",
//@              "apiVersion": "2020-01-01",
//@              "name": "basicblobs",
//@              "location": "[parameters('stringParamA')]"
//@            },
//@            {
//@              "type": "Mock.Rp/mockResource",
//@              "apiVersion": "2020-01-01",
//@              "name": "myZone",
//@              "location": "[parameters('stringParamB')]"
//@            }
//@          ],
//@          "outputs": {
//@            "stringOutputA": {
//@              "type": "string",
//@              "value": "[parameters('stringParamA')]"
//@            },
//@            "stringOutputB": {
//@              "type": "string",
//@              "value": "[parameters('stringParamB')]"
//@            },
//@            "objOutput": {
//@              "type": "object",
//@              "value": "[reference(resourceId('Mock.Rp/mockResource', 'basicblobs'), '2020-01-01')]"
//@            },
//@            "arrayOutput": {
//@              "type": "array",
//@              "value": [
//@                "[resourceId('Mock.Rp/mockResource', 'basicblobs')]",
//@                "[resourceId('Mock.Rp/mockResource', 'myZone')]"
//@              ]
//@            }
//@          }
//@        }
//@      }
//@    },
  name: module.name
//@      "name": "[variables('myModules')[copyIndex()].name]",
  params: {
    arrayParam: [for i in range(0,3): concat('test-', i, '-', module.name)]
//@              {
//@                "name": "value",
//@                "count": "[length(range(0, 3))]",
//@                "input": "[concat('test-', range(0, 3)[copyIndex('value')], '-', variables('myModules')[copyIndex()].name)]"
//@              }
    objParam: module
//@            "value": "[variables('myModules')[copyIndex()]]"
    stringParamB: module.location
//@            "value": "[variables('myModules')[copyIndex()].location]"
  }
}]

// duplicate identifiers across scopes are allowed (inner hides the outer)
module duplicateIdentifiersWithinLoop 'modulea.bicep' = [for x in emptyArray:{
//@    {
//@      "copy": {
//@        "name": "duplicateIdentifiersWithinLoop",
//@        "count": "[length(variables('emptyArray'))]"
//@      },
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2020-10-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "parameters": {
//@          "objParam": {
//@          },
//@          "stringParamA": {
//@          },
//@          "stringParamB": {
//@          },
//@          "arrayParam": {
//@            "copy": [
//@            ]
//@          }
//@        },
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "8300391961099598421"
//@            }
//@          },
//@          "parameters": {
//@            "stringParamA": {
//@              "type": "string",
//@              "defaultValue": "test"
//@            },
//@            "stringParamB": {
//@              "type": "string"
//@            },
//@            "objParam": {
//@              "type": "object"
//@            },
//@            "arrayParam": {
//@              "type": "array"
//@            }
//@          },
//@          "resources": [
//@            {
//@              "type": "Mock.Rp/mockResource",
//@              "apiVersion": "2020-01-01",
//@              "name": "basicblobs",
//@              "location": "[parameters('stringParamA')]"
//@            },
//@            {
//@              "type": "Mock.Rp/mockResource",
//@              "apiVersion": "2020-01-01",
//@              "name": "myZone",
//@              "location": "[parameters('stringParamB')]"
//@            }
//@          ],
//@          "outputs": {
//@            "stringOutputA": {
//@              "type": "string",
//@              "value": "[parameters('stringParamA')]"
//@            },
//@            "stringOutputB": {
//@              "type": "string",
//@              "value": "[parameters('stringParamB')]"
//@            },
//@            "objOutput": {
//@              "type": "object",
//@              "value": "[reference(resourceId('Mock.Rp/mockResource', 'basicblobs'), '2020-01-01')]"
//@            },
//@            "arrayOutput": {
//@              "type": "array",
//@              "value": [
//@                "[resourceId('Mock.Rp/mockResource', 'basicblobs')]",
//@                "[resourceId('Mock.Rp/mockResource', 'myZone')]"
//@              ]
//@            }
//@          }
//@        }
//@      }
//@    },
  name: 'hello-${x}'
//@      "name": "[format('hello-{0}', variables('emptyArray')[copyIndex()])]",
  params: {
    objParam: {}
//@            "value": {}
    stringParamA: 'test'
//@            "value": "test"
    stringParamB: 'test'
//@            "value": "test"
    arrayParam: [for x in emptyArray: x]
//@              {
//@                "name": "value",
//@                "count": "[length(variables('emptyArray'))]",
//@                "input": "[variables('emptyArray')[copyIndex('value')]]"
//@              }
  }
}]

// duplicate identifiers across scopes are allowed (inner hides the outer)
var duplicateAcrossScopes = 'hello'
//@    "duplicateAcrossScopes": "hello",
module duplicateInGlobalAndOneLoop 'modulea.bicep' = [for duplicateAcrossScopes in []: {
//@    {
//@      "copy": {
//@        "name": "duplicateInGlobalAndOneLoop",
//@        "count": "[length(createArray())]"
//@      },
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2020-10-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "parameters": {
//@          "objParam": {
//@          },
//@          "stringParamA": {
//@          },
//@          "stringParamB": {
//@          },
//@          "arrayParam": {
//@            "copy": [
//@            ]
//@          }
//@        },
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "8300391961099598421"
//@            }
//@          },
//@          "parameters": {
//@            "stringParamA": {
//@              "type": "string",
//@              "defaultValue": "test"
//@            },
//@            "stringParamB": {
//@              "type": "string"
//@            },
//@            "objParam": {
//@              "type": "object"
//@            },
//@            "arrayParam": {
//@              "type": "array"
//@            }
//@          },
//@          "resources": [
//@            {
//@              "type": "Mock.Rp/mockResource",
//@              "apiVersion": "2020-01-01",
//@              "name": "basicblobs",
//@              "location": "[parameters('stringParamA')]"
//@            },
//@            {
//@              "type": "Mock.Rp/mockResource",
//@              "apiVersion": "2020-01-01",
//@              "name": "myZone",
//@              "location": "[parameters('stringParamB')]"
//@            }
//@          ],
//@          "outputs": {
//@            "stringOutputA": {
//@              "type": "string",
//@              "value": "[parameters('stringParamA')]"
//@            },
//@            "stringOutputB": {
//@              "type": "string",
//@              "value": "[parameters('stringParamB')]"
//@            },
//@            "objOutput": {
//@              "type": "object",
//@              "value": "[reference(resourceId('Mock.Rp/mockResource', 'basicblobs'), '2020-01-01')]"
//@            },
//@            "arrayOutput": {
//@              "type": "array",
//@              "value": [
//@                "[resourceId('Mock.Rp/mockResource', 'basicblobs')]",
//@                "[resourceId('Mock.Rp/mockResource', 'myZone')]"
//@              ]
//@            }
//@          }
//@        }
//@      }
//@    },
  name: 'hello-${duplicateAcrossScopes}'
//@      "name": "[format('hello-{0}', createArray()[copyIndex()])]",
  params: {
    objParam: {}
//@            "value": {}
    stringParamA: 'test'
//@            "value": "test"
    stringParamB: 'test'
//@            "value": "test"
    arrayParam: [for x in emptyArray: x]
//@              {
//@                "name": "value",
//@                "count": "[length(variables('emptyArray'))]",
//@                "input": "[variables('emptyArray')[copyIndex('value')]]"
//@              }
  }
}]

var someDuplicate = true
//@    "someDuplicate": true,
var otherDuplicate = false
//@    "otherDuplicate": false,
module duplicatesEverywhere 'modulea.bicep' = [for someDuplicate in []: {
//@    {
//@      "copy": {
//@        "name": "duplicatesEverywhere",
//@        "count": "[length(createArray())]"
//@      },
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2020-10-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "parameters": {
//@          "objParam": {
//@          },
//@          "stringParamB": {
//@          },
//@          "arrayParam": {
//@            "copy": [
//@            ]
//@          }
//@        },
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "8300391961099598421"
//@            }
//@          },
//@          "parameters": {
//@            "stringParamA": {
//@              "type": "string",
//@              "defaultValue": "test"
//@            },
//@            "stringParamB": {
//@              "type": "string"
//@            },
//@            "objParam": {
//@              "type": "object"
//@            },
//@            "arrayParam": {
//@              "type": "array"
//@            }
//@          },
//@          "resources": [
//@            {
//@              "type": "Mock.Rp/mockResource",
//@              "apiVersion": "2020-01-01",
//@              "name": "basicblobs",
//@              "location": "[parameters('stringParamA')]"
//@            },
//@            {
//@              "type": "Mock.Rp/mockResource",
//@              "apiVersion": "2020-01-01",
//@              "name": "myZone",
//@              "location": "[parameters('stringParamB')]"
//@            }
//@          ],
//@          "outputs": {
//@            "stringOutputA": {
//@              "type": "string",
//@              "value": "[parameters('stringParamA')]"
//@            },
//@            "stringOutputB": {
//@              "type": "string",
//@              "value": "[parameters('stringParamB')]"
//@            },
//@            "objOutput": {
//@              "type": "object",
//@              "value": "[reference(resourceId('Mock.Rp/mockResource', 'basicblobs'), '2020-01-01')]"
//@            },
//@            "arrayOutput": {
//@              "type": "array",
//@              "value": [
//@                "[resourceId('Mock.Rp/mockResource', 'basicblobs')]",
//@                "[resourceId('Mock.Rp/mockResource', 'myZone')]"
//@              ]
//@            }
//@          }
//@        }
//@      }
//@    },
  name: 'hello-${someDuplicate}'
//@      "name": "[format('hello-{0}', createArray()[copyIndex()])]",
  params: {
    objParam: {}
//@            "value": {}
    stringParamB: 'test'
//@            "value": "test"
    arrayParam: [for otherDuplicate in emptyArray: '${someDuplicate}-${otherDuplicate}']
//@              {
//@                "name": "value",
//@                "count": "[length(variables('emptyArray'))]",
//@                "input": "[format('{0}-{1}', createArray()[copyIndex()], variables('emptyArray')[copyIndex('value')])]"
//@              }
  }
}]

module propertyLoopInsideParameterValue 'modulea.bicep' = {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2020-10-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "parameters": {
//@          "objParam": {
//@          },
//@          "stringParamB": {
//@          },
//@          "arrayParam": {
//@          }
//@        },
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "8300391961099598421"
//@            }
//@          },
//@          "parameters": {
//@            "stringParamA": {
//@              "type": "string",
//@              "defaultValue": "test"
//@            },
//@            "stringParamB": {
//@              "type": "string"
//@            },
//@            "objParam": {
//@              "type": "object"
//@            },
//@            "arrayParam": {
//@              "type": "array"
//@            }
//@          },
//@          "resources": [
//@            {
//@              "type": "Mock.Rp/mockResource",
//@              "apiVersion": "2020-01-01",
//@              "name": "basicblobs",
//@              "location": "[parameters('stringParamA')]"
//@            },
//@            {
//@              "type": "Mock.Rp/mockResource",
//@              "apiVersion": "2020-01-01",
//@              "name": "myZone",
//@              "location": "[parameters('stringParamB')]"
//@            }
//@          ],
//@          "outputs": {
//@            "stringOutputA": {
//@              "type": "string",
//@              "value": "[parameters('stringParamA')]"
//@            },
//@            "stringOutputB": {
//@              "type": "string",
//@              "value": "[parameters('stringParamB')]"
//@            },
//@            "objOutput": {
//@              "type": "object",
//@              "value": "[reference(resourceId('Mock.Rp/mockResource', 'basicblobs'), '2020-01-01')]"
//@            },
//@            "arrayOutput": {
//@              "type": "array",
//@              "value": [
//@                "[resourceId('Mock.Rp/mockResource', 'basicblobs')]",
//@                "[resourceId('Mock.Rp/mockResource', 'myZone')]"
//@              ]
//@            }
//@          }
//@        }
//@      }
//@    },
  name: 'propertyLoopInsideParameterValue'
//@      "name": "propertyLoopInsideParameterValue",
  params: {
    objParam: {
//@            "value": {
//@              "copy": [
//@              ],
//@            }
      a: [for i in range(0,10): i]
//@                {
//@                  "name": "a",
//@                  "count": "[length(range(0, 10))]",
//@                  "input": "[range(0, 10)[copyIndex('a')]]"
//@                },
      b: [for i in range(1,2): i]
//@                {
//@                  "name": "b",
//@                  "count": "[length(range(1, 2))]",
//@                  "input": "[range(1, 2)[copyIndex('b')]]"
//@                },
      c: {
//@              "c": {
//@                "copy": [
//@                ]
//@              }
        d: [for j in range(2,3): j]
//@                  {
//@                    "name": "d",
//@                    "count": "[length(range(2, 3))]",
//@                    "input": "[range(2, 3)[copyIndex('d')]]"
//@                  }
      }
      e: [for k in range(4,4): {
//@                {
//@                  "name": "e",
//@                  "count": "[length(range(4, 4))]",
//@                  "input": {
//@                  }
//@                }
        f: k
//@                    "f": "[range(4, 4)[copyIndex('e')]]"
      }]
    }
    stringParamB: ''
//@            "value": ""
    arrayParam: [
//@            "value": [
//@            ]
      {
//@              {
//@                "copy": [
//@                ]
//@              }
        e: [for j in range(7,7): j]
//@                  {
//@                    "name": "e",
//@                    "count": "[length(range(7, 7))]",
//@                    "input": "[range(7, 7)[copyIndex('e')]]"
//@                  }
      }
    ]
  }
}

module propertyLoopInsideParameterValueWithIndexes 'modulea.bicep' = {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2020-10-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "parameters": {
//@          "objParam": {
//@          },
//@          "stringParamB": {
//@          },
//@          "arrayParam": {
//@          }
//@        },
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "8300391961099598421"
//@            }
//@          },
//@          "parameters": {
//@            "stringParamA": {
//@              "type": "string",
//@              "defaultValue": "test"
//@            },
//@            "stringParamB": {
//@              "type": "string"
//@            },
//@            "objParam": {
//@              "type": "object"
//@            },
//@            "arrayParam": {
//@              "type": "array"
//@            }
//@          },
//@          "resources": [
//@            {
//@              "type": "Mock.Rp/mockResource",
//@              "apiVersion": "2020-01-01",
//@              "name": "basicblobs",
//@              "location": "[parameters('stringParamA')]"
//@            },
//@            {
//@              "type": "Mock.Rp/mockResource",
//@              "apiVersion": "2020-01-01",
//@              "name": "myZone",
//@              "location": "[parameters('stringParamB')]"
//@            }
//@          ],
//@          "outputs": {
//@            "stringOutputA": {
//@              "type": "string",
//@              "value": "[parameters('stringParamA')]"
//@            },
//@            "stringOutputB": {
//@              "type": "string",
//@              "value": "[parameters('stringParamB')]"
//@            },
//@            "objOutput": {
//@              "type": "object",
//@              "value": "[reference(resourceId('Mock.Rp/mockResource', 'basicblobs'), '2020-01-01')]"
//@            },
//@            "arrayOutput": {
//@              "type": "array",
//@              "value": [
//@                "[resourceId('Mock.Rp/mockResource', 'basicblobs')]",
//@                "[resourceId('Mock.Rp/mockResource', 'myZone')]"
//@              ]
//@            }
//@          }
//@        }
//@      }
//@    },
  name: 'propertyLoopInsideParameterValueWithIndexes'
//@      "name": "propertyLoopInsideParameterValueWithIndexes",
  params: {
    objParam: {
//@            "value": {
//@              "copy": [
//@              ],
//@            }
      a: [for (i, i2) in range(0,10): i + i2]
//@                {
//@                  "name": "a",
//@                  "count": "[length(range(0, 10))]",
//@                  "input": "[add(range(0, 10)[copyIndex('a')], copyIndex('a'))]"
//@                },
      b: [for (i, i2) in range(1,2): i / i2]
//@                {
//@                  "name": "b",
//@                  "count": "[length(range(1, 2))]",
//@                  "input": "[div(range(1, 2)[copyIndex('b')], copyIndex('b'))]"
//@                },
      c: {
//@              "c": {
//@                "copy": [
//@                ]
//@              }
        d: [for (j, j2) in range(2,3): j * j2]
//@                  {
//@                    "name": "d",
//@                    "count": "[length(range(2, 3))]",
//@                    "input": "[mul(range(2, 3)[copyIndex('d')], copyIndex('d'))]"
//@                  }
      }
      e: [for (k, k2) in range(4,4): {
//@                {
//@                  "name": "e",
//@                  "count": "[length(range(4, 4))]",
//@                  "input": {
//@                  }
//@                }
        f: k
//@                    "f": "[range(4, 4)[copyIndex('e')]]",
        g: k2
//@                    "g": "[copyIndex('e')]"
      }]
    }
    stringParamB: ''
//@            "value": ""
    arrayParam: [
//@            "value": [
//@            ]
      {
//@              {
//@                "copy": [
//@                ]
//@              }
        e: [for j in range(7,7): j]
//@                  {
//@                    "name": "e",
//@                    "count": "[length(range(7, 7))]",
//@                    "input": "[range(7, 7)[copyIndex('e')]]"
//@                  }
      }
    ]
  }
}

module propertyLoopInsideParameterValueInsideModuleLoop 'modulea.bicep' = [for thing in range(0,1): {
//@    {
//@      "copy": {
//@        "name": "propertyLoopInsideParameterValueInsideModuleLoop",
//@        "count": "[length(range(0, 1))]"
//@      },
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2020-10-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "parameters": {
//@          "objParam": {
//@          },
//@          "stringParamB": {
//@          },
//@          "arrayParam": {
//@          }
//@        },
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "8300391961099598421"
//@            }
//@          },
//@          "parameters": {
//@            "stringParamA": {
//@              "type": "string",
//@              "defaultValue": "test"
//@            },
//@            "stringParamB": {
//@              "type": "string"
//@            },
//@            "objParam": {
//@              "type": "object"
//@            },
//@            "arrayParam": {
//@              "type": "array"
//@            }
//@          },
//@          "resources": [
//@            {
//@              "type": "Mock.Rp/mockResource",
//@              "apiVersion": "2020-01-01",
//@              "name": "basicblobs",
//@              "location": "[parameters('stringParamA')]"
//@            },
//@            {
//@              "type": "Mock.Rp/mockResource",
//@              "apiVersion": "2020-01-01",
//@              "name": "myZone",
//@              "location": "[parameters('stringParamB')]"
//@            }
//@          ],
//@          "outputs": {
//@            "stringOutputA": {
//@              "type": "string",
//@              "value": "[parameters('stringParamA')]"
//@            },
//@            "stringOutputB": {
//@              "type": "string",
//@              "value": "[parameters('stringParamB')]"
//@            },
//@            "objOutput": {
//@              "type": "object",
//@              "value": "[reference(resourceId('Mock.Rp/mockResource', 'basicblobs'), '2020-01-01')]"
//@            },
//@            "arrayOutput": {
//@              "type": "array",
//@              "value": [
//@                "[resourceId('Mock.Rp/mockResource', 'basicblobs')]",
//@                "[resourceId('Mock.Rp/mockResource', 'myZone')]"
//@              ]
//@            }
//@          }
//@        }
//@      }
//@    },
  name: 'propertyLoopInsideParameterValueInsideModuleLoop'
//@      "name": "propertyLoopInsideParameterValueInsideModuleLoop",
  params: {
    objParam: {
//@            "value": {
//@              "copy": [
//@              ],
//@            }
      a: [for i in range(0,10): i + thing]
//@                {
//@                  "name": "a",
//@                  "count": "[length(range(0, 10))]",
//@                  "input": "[add(range(0, 10)[copyIndex('a')], range(0, 1)[copyIndex()])]"
//@                },
      b: [for i in range(1,2): i * thing]
//@                {
//@                  "name": "b",
//@                  "count": "[length(range(1, 2))]",
//@                  "input": "[mul(range(1, 2)[copyIndex('b')], range(0, 1)[copyIndex()])]"
//@                },
      c: {
//@              "c": {
//@                "copy": [
//@                ]
//@              }
        d: [for j in range(2,3): j]
//@                  {
//@                    "name": "d",
//@                    "count": "[length(range(2, 3))]",
//@                    "input": "[range(2, 3)[copyIndex('d')]]"
//@                  }
      }
      e: [for k in range(4,4): {
//@                {
//@                  "name": "e",
//@                  "count": "[length(range(4, 4))]",
//@                  "input": {
//@                  }
//@                }
        f: k - thing
//@                    "f": "[sub(range(4, 4)[copyIndex('e')], range(0, 1)[copyIndex()])]"
      }]
    }
    stringParamB: ''
//@            "value": ""
    arrayParam: [
//@            "value": [
//@            ]
      {
//@              {
//@                "copy": [
//@                ]
//@              }
        e: [for j in range(7,7): j % thing]
//@                  {
//@                    "name": "e",
//@                    "count": "[length(range(7, 7))]",
//@                    "input": "[mod(range(7, 7)[copyIndex('e')], range(0, 1)[copyIndex()])]"
//@                  }
      }
    ]
  }
}]


// BEGIN: Key Vault Secret Reference

resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
}

module secureModule1 'child/secureParams.bicep' = {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2020-10-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "parameters": {
//@          "secureStringParam1": {
//@            "reference": {
//@              "keyVault": {
//@                "id": "[resourceId('Microsoft.KeyVault/vaults', 'testkeyvault')]"
//@              },
//@              "secretName": "mySecret"
//@            }
//@          },
//@          "secureStringParam2": {
//@            "reference": {
//@              "keyVault": {
//@                "id": "[resourceId('Microsoft.KeyVault/vaults', 'testkeyvault')]"
//@              },
//@              "secretName": "mySecret",
//@              "secretVersion": "secretVersion"
//@            }
//@          }
//@        },
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "15522334618541518671"
//@            }
//@          },
//@          "parameters": {
//@            "secureStringParam1": {
//@              "type": "securestring"
//@            },
//@            "secureStringParam2": {
//@              "type": "securestring",
//@              "defaultValue": ""
//@            }
//@          },
//@          "resources": [],
//@          "outputs": {
//@            "exposedSecureString": {
//@              "type": "string",
//@              "value": "[parameters('secureStringParam1')]"
//@            }
//@          }
//@        }
//@      }
//@    },
  name: 'secureModule1'
//@      "name": "secureModule1",
  params: {
    secureStringParam1: kv.getSecret('mySecret')
    secureStringParam2: kv.getSecret('mySecret','secretVersion')
  }
}

resource scopedKv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
  scope: resourceGroup('otherGroup')
}

module secureModule2 'child/secureParams.bicep' = {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2020-10-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "parameters": {
//@          "secureStringParam1": {
//@            "reference": {
//@              "keyVault": {
//@                "id": "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'otherGroup'), 'Microsoft.KeyVault/vaults', 'testkeyvault')]"
//@              },
//@              "secretName": "mySecret"
//@            }
//@          },
//@          "secureStringParam2": {
//@            "reference": {
//@              "keyVault": {
//@                "id": "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'otherGroup'), 'Microsoft.KeyVault/vaults', 'testkeyvault')]"
//@              },
//@              "secretName": "mySecret",
//@              "secretVersion": "secretVersion"
//@            }
//@          }
//@        },
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "15522334618541518671"
//@            }
//@          },
//@          "parameters": {
//@            "secureStringParam1": {
//@              "type": "securestring"
//@            },
//@            "secureStringParam2": {
//@              "type": "securestring",
//@              "defaultValue": ""
//@            }
//@          },
//@          "resources": [],
//@          "outputs": {
//@            "exposedSecureString": {
//@              "type": "string",
//@              "value": "[parameters('secureStringParam1')]"
//@            }
//@          }
//@        }
//@      }
//@    },
  name: 'secureModule2'
//@      "name": "secureModule2",
  params: {
    secureStringParam1: scopedKv.getSecret('mySecret')
    secureStringParam2: scopedKv.getSecret('mySecret','secretVersion')
  }
}

//looped module with looped existing resource (Issue #2862)
var vaults = [
//@    "vaults": [
//@    ],
  {
//@      {
//@      },
    vaultName: 'test-1-kv'
//@        "vaultName": "test-1-kv",
    vaultRG: 'test-1-rg'
//@        "vaultRG": "test-1-rg",
    vaultSub: 'abcd-efgh'
//@        "vaultSub": "abcd-efgh"
  }
  {
//@      {
//@      }
    vaultName: 'test-2-kv'
//@        "vaultName": "test-2-kv",
    vaultRG: 'test-2-rg'
//@        "vaultRG": "test-2-rg",
    vaultSub: 'ijkl-1adg1'
//@        "vaultSub": "ijkl-1adg1"
  }
]
var secrets = [
//@    "secrets": [
//@    ]
  {
//@      {
//@      },
    name: 'secret01'
//@        "name": "secret01",
    version: 'versionA'
//@        "version": "versionA"
  }
  {
//@      {
//@      }
    name: 'secret02'
//@        "name": "secret02",
    version: 'versionB'
//@        "version": "versionB"
  }
]

resource loopedKv 'Microsoft.KeyVault/vaults@2019-09-01' existing = [for vault in vaults: {
  name: vault.vaultName
  scope: resourceGroup(vault.vaultSub, vault.vaultRG)
}]

module secureModuleLooped 'child/secureParams.bicep' = [for (secret, i) in secrets: {
//@    {
//@      "copy": {
//@        "name": "secureModuleLooped",
//@        "count": "[length(variables('secrets'))]"
//@      },
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2020-10-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "parameters": {
//@          "secureStringParam1": {
//@            "reference": {
//@              "keyVault": {
//@                "id": "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', variables('vaults')[copyIndex()].vaultSub, variables('vaults')[copyIndex()].vaultRG), 'Microsoft.KeyVault/vaults', variables('vaults')[copyIndex()].vaultName)]"
//@              },
//@              "secretName": "[variables('secrets')[copyIndex()].name]"
//@            }
//@          },
//@          "secureStringParam2": {
//@            "reference": {
//@              "keyVault": {
//@                "id": "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', variables('vaults')[copyIndex()].vaultSub, variables('vaults')[copyIndex()].vaultRG), 'Microsoft.KeyVault/vaults', variables('vaults')[copyIndex()].vaultName)]"
//@              },
//@              "secretName": "[variables('secrets')[copyIndex()].name]",
//@              "secretVersion": "[variables('secrets')[copyIndex()].version]"
//@            }
//@          }
//@        },
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "15522334618541518671"
//@            }
//@          },
//@          "parameters": {
//@            "secureStringParam1": {
//@              "type": "securestring"
//@            },
//@            "secureStringParam2": {
//@              "type": "securestring",
//@              "defaultValue": ""
//@            }
//@          },
//@          "resources": [],
//@          "outputs": {
//@            "exposedSecureString": {
//@              "type": "string",
//@              "value": "[parameters('secureStringParam1')]"
//@            }
//@          }
//@        }
//@      }
//@    },
  name: 'secureModuleLooped-${i}'
//@      "name": "[format('secureModuleLooped-{0}', copyIndex())]",
  params: {
    secureStringParam1: loopedKv[i].getSecret(secret.name)
    secureStringParam2: loopedKv[i].getSecret(secret.name, secret.version)
  }
}]

module secureModuleCondition 'child/secureParams.bicep' = {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2020-10-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "parameters": {
//@          "secureStringParam1": "[if(true(), createObject('reference', createObject('keyVault', createObject('id', resourceId('Microsoft.KeyVault/vaults', 'testkeyvault')), 'secretName', 'mySecret')), createObject('value', 'notTrue'))]",
//@          "secureStringParam2": "[if(true(), if(false(), createObject('value', 'false'), createObject('reference', createObject('keyVault', createObject('id', resourceId('Microsoft.KeyVault/vaults', 'testkeyvault')), 'secretName', 'mySecret', 'secretVersion', 'secretVersion'))), createObject('value', 'notTrue'))]"
//@        },
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "15522334618541518671"
//@            }
//@          },
//@          "parameters": {
//@            "secureStringParam1": {
//@              "type": "securestring"
//@            },
//@            "secureStringParam2": {
//@              "type": "securestring",
//@              "defaultValue": ""
//@            }
//@          },
//@          "resources": [],
//@          "outputs": {
//@            "exposedSecureString": {
//@              "type": "string",
//@              "value": "[parameters('secureStringParam1')]"
//@            }
//@          }
//@        }
//@      }
//@    },
  name: 'secureModuleCondition'
//@      "name": "secureModuleCondition",
  params: {
    secureStringParam1: true ? kv.getSecret('mySecret') : 'notTrue'
    secureStringParam2: true ? false ? 'false' : kv.getSecret('mySecret','secretVersion') : 'notTrue'
  }
}

// END: Key Vault Secret Reference

module withSpace 'module with space.bicep' = {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2020-10-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "1347091426241151379"
//@            }
//@          },
//@          "parameters": {
//@            "location": {
//@              "type": "string",
//@              "defaultValue": "westus"
//@            }
//@          },
//@          "resources": [],
//@          "outputs": {
//@            "loc": {
//@              "type": "string",
//@              "value": "[parameters('location')]"
//@            }
//@          }
//@        }
//@      }
//@    },
  name: 'withSpace'
//@      "name": "withSpace",
}

module folderWithSpace 'child/folder with space/child with space.bicep' = {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2020-10-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "1347091426241151379"
//@            }
//@          },
//@          "parameters": {
//@            "location": {
//@              "type": "string",
//@              "defaultValue": "westus"
//@            }
//@          },
//@          "resources": [],
//@          "outputs": {
//@            "loc": {
//@              "type": "string",
//@              "value": "[parameters('location')]"
//@            }
//@          }
//@        }
//@      }
//@    },
  name: 'childWithSpace'
//@      "name": "childWithSpace",
}

module withSeparateConfig './child/folder with separate config/moduleWithAzImport.bicep' = {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2020-10-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "languageVersion": "1.9-experimental",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_EXPERIMENTAL_WARNING": "Symbolic name support in ARM is experimental, and should be enabled for testing purposes only. Do not enable this setting for any production usage, or you may be unexpectedly broken at any time!",
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "4117709985067600940"
//@            }
//@          },
//@          "imports": {
//@            "az": {
//@              "provider": "AzureResourceManager",
//@              "version": "1.0.0"
//@            }
//@          },
//@          "resources": {},
//@          "outputs": {
//@            "str": {
//@              "type": "string",
//@              "value": "foo"
//@            }
//@          }
//@        }
//@      }
//@    }
  name: 'withSeparateConfig'
//@      "name": "withSeparateConfig",
}

