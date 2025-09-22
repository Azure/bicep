
@sys.description('this is deployTimeSuffix param')
//@      "metadata": {
//@        "description": "this is deployTimeSuffix param"
//@      }
param deployTimeSuffix string = newGuid()
//@    "deployTimeSuffix": {
//@      "type": "string",
//@      "defaultValue": "[newGuid()]",
//@    }

@sys.description('this module a')
//@        "description": "this module a"
module modATest './modulea.bicep' = {
//@    "modATest": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
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
//@        "parameters": {
//@        },
    stringParamB: 'hello!'
//@          "stringParamB": {
//@            "value": "hello!"
//@          },
    objParam: {
//@          "objParam": {
//@            "value": {
//@            }
//@          },
      a: 'b'
//@              "a": "b"
    }
    arrayParam: [
//@          "arrayParam": {
//@            "value": [
//@            ]
//@          }
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
//@    "modB": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
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
//@        "parameters": {
//@        },
    location: 'West US'
//@          "location": {
//@            "value": "West US"
//@          }
  }
}

@sys.description('this is just module b with a condition')
//@        "description": "this is just module b with a condition"
module modBWithCondition './child/moduleb.bicep' = if (1 + 1 == 2) {
//@    "modBWithCondition": {
//@      "condition": "[equals(add(1, 1), 2)]",
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
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
//@        "parameters": {
//@        },
    location: 'East US'
//@          "location": {
//@            "value": "East US"
//@          }
  }
}

module modBWithCondition2 './child/moduleb.bicep' =
//@    "modBWithCondition2": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
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
//@      }
//@    },
// awkward comment
if (1 + 1 == 2) {
//@      "condition": "[equals(add(1, 1), 2)]",
  name: 'modBWithCondition2'
//@      "name": "modBWithCondition2",
  params: {
//@        "parameters": {
//@        },
    location: 'East US'
//@          "location": {
//@            "value": "East US"
//@          }
  }
}

module modC './child/modulec.json' = {
//@    "modC": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
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
//@        "parameters": {
//@        },
    location: 'West US'
//@          "location": {
//@            "value": "West US"
//@          }
  }
}

module modCWithCondition './child/modulec.json' = if (2 - 1 == 1) {
//@    "modCWithCondition": {
//@      "condition": "[equals(sub(2, 1), 1)]",
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
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
//@        "parameters": {
//@        },
    location: 'East US'
//@          "location": {
//@            "value": "East US"
//@          }
  }
}

module optionalWithNoParams1 './child/optionalParams.bicep'= {
//@    "optionalWithNoParams1": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
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
//@    "optionalWithNoParams2": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
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
  name: 'optionalWithNoParams2'
//@      "name": "optionalWithNoParams2",
  params: {
//@        "parameters": {},
  }
}

module optionalWithAllParams './child/optionalParams.bicep'= {
//@    "optionalWithAllParams": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
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
  name: 'optionalWithNoParams3'
//@      "name": "optionalWithNoParams3",
  params: {
//@        "parameters": {
//@        },
    optionalString: 'abc'
//@          "optionalString": {
//@            "value": "abc"
//@          },
    optionalInt: 42
//@          "optionalInt": {
//@            "value": 42
//@          },
    optionalObj: { }
//@          "optionalObj": {
//@            "value": {}
//@          },
    optionalArray: [ ]
//@          "optionalArray": {
//@            "value": []
//@          }
  }
}

resource resWithDependencies 'Mock.Rp/mockResource@2020-01-01' = {
//@    "resWithDependencies": {
//@      "type": "Mock.Rp/mockResource",
//@      "apiVersion": "2020-01-01",
//@      "name": "harry",
//@      "dependsOn": [
//@        "modATest",
//@        "modB",
//@        "modC"
//@      ]
//@    },
  name: 'harry'
  properties: {
//@      "properties": {
//@      },
    modADep: modATest.outputs.stringOutputA
//@        "modADep": "[reference('modATest').outputs.stringOutputA.value]",
    modBDep: modB.outputs.myResourceId
//@        "modBDep": "[reference('modB').outputs.myResourceId.value]",
    modCDep: modC.outputs.myResourceId
//@        "modCDep": "[reference('modC').outputs.myResourceId.value]"
  }
}

module optionalWithAllParamsAndManualDependency './child/optionalParams.bicep'= {
//@    "optionalWithAllParamsAndManualDependency": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
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
//@      },
//@      "dependsOn": [
//@        "optionalWithAllParams",
//@        "resWithDependencies"
//@      ]
//@    },
  name: 'optionalWithAllParamsAndManualDependency'
//@      "name": "optionalWithAllParamsAndManualDependency",
  params: {
//@        "parameters": {
//@        },
    optionalString: 'abc'
//@          "optionalString": {
//@            "value": "abc"
//@          },
    optionalInt: 42
//@          "optionalInt": {
//@            "value": 42
//@          },
    optionalObj: { }
//@          "optionalObj": {
//@            "value": {}
//@          },
    optionalArray: [ ]
//@          "optionalArray": {
//@            "value": []
//@          }
  }
  dependsOn: [
    resWithDependencies
    optionalWithAllParams
  ]
}

module optionalWithImplicitDependency './child/optionalParams.bicep'= {
//@    "optionalWithImplicitDependency": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
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
//@      },
//@      "dependsOn": [
//@        "optionalWithAllParamsAndManualDependency",
//@        "resWithDependencies"
//@      ]
//@    },
  name: 'optionalWithImplicitDependency'
//@      "name": "optionalWithImplicitDependency",
  params: {
//@        "parameters": {
//@        },
    optionalString: concat(resWithDependencies.id, optionalWithAllParamsAndManualDependency.name)
//@          "optionalString": {
//@            "value": "[concat(resourceId('Mock.Rp/mockResource', 'harry'), 'optionalWithAllParamsAndManualDependency')]"
//@          },
    optionalInt: 42
//@          "optionalInt": {
//@            "value": 42
//@          },
    optionalObj: { }
//@          "optionalObj": {
//@            "value": {}
//@          },
    optionalArray: [ ]
//@          "optionalArray": {
//@            "value": []
//@          }
  }
}

module moduleWithCalculatedName './child/optionalParams.bicep'= {
//@    "moduleWithCalculatedName": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
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
//@      },
//@      "dependsOn": [
//@        "optionalWithAllParamsAndManualDependency",
//@        "resWithDependencies"
//@      ]
//@    },
  name: '${optionalWithAllParamsAndManualDependency.name}${deployTimeSuffix}'
//@      "name": "[format('{0}{1}', 'optionalWithAllParamsAndManualDependency', parameters('deployTimeSuffix'))]",
  params: {
//@        "parameters": {
//@        },
    optionalString: concat(resWithDependencies.id, optionalWithAllParamsAndManualDependency.name)
//@          "optionalString": {
//@            "value": "[concat(resourceId('Mock.Rp/mockResource', 'harry'), 'optionalWithAllParamsAndManualDependency')]"
//@          },
    optionalInt: 42
//@          "optionalInt": {
//@            "value": 42
//@          },
    optionalObj: { }
//@          "optionalObj": {
//@            "value": {}
//@          },
    optionalArray: [ ]
//@          "optionalArray": {
//@            "value": []
//@          }
  }
}

resource resWithCalculatedNameDependencies 'Mock.Rp/mockResource@2020-01-01' = {
//@    "resWithCalculatedNameDependencies": {
//@      "type": "Mock.Rp/mockResource",
//@      "apiVersion": "2020-01-01",
//@      "name": "[format('{0}{1}', 'optionalWithAllParamsAndManualDependency', parameters('deployTimeSuffix'))]",
//@      "dependsOn": [
//@        "moduleWithCalculatedName",
//@        "optionalWithAllParamsAndManualDependency"
//@      ]
//@    },
  name: '${optionalWithAllParamsAndManualDependency.name}${deployTimeSuffix}'
  properties: {
//@      "properties": {
//@      },
    modADep: moduleWithCalculatedName.outputs.outputObj
//@        "modADep": "[reference('moduleWithCalculatedName').outputs.outputObj.value]"
  }
}

output stringOutputA string = modATest.outputs.stringOutputA
//@    "stringOutputA": {
//@      "type": "string",
//@      "value": "[reference('modATest').outputs.stringOutputA.value]"
//@    },
output stringOutputB string = modATest.outputs.stringOutputB
//@    "stringOutputB": {
//@      "type": "string",
//@      "value": "[reference('modATest').outputs.stringOutputB.value]"
//@    },
output objOutput object = modATest.outputs.objOutput
//@    "objOutput": {
//@      "type": "object",
//@      "value": "[reference('modATest').outputs.objOutput.value]"
//@    },
output arrayOutput array = modATest.outputs.arrayOutput
//@    "arrayOutput": {
//@      "type": "array",
//@      "value": "[reference('modATest').outputs.arrayOutput.value]"
//@    },
output modCalculatedNameOutput object = moduleWithCalculatedName.outputs.outputObj
//@    "modCalculatedNameOutput": {
//@      "type": "object",
//@      "value": "[reference('moduleWithCalculatedName').outputs.outputObj.value]"
//@    },

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
//@    "storageResources": {
//@      "copy": {
//@        "name": "storageResources",
//@        "count": "[length(variables('myModules'))]"
//@      },
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
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
//@        "parameters": {
//@        },
    arrayParam: []
//@          "arrayParam": {
//@            "value": []
//@          },
    objParam: module
//@          "objParam": {
//@            "value": "[variables('myModules')[copyIndex()]]"
//@          },
    stringParamB: module.location
//@          "stringParamB": {
//@            "value": "[variables('myModules')[copyIndex()].location]"
//@          }
  }
}]

// simple indexed module loop
module storageResourcesWithIndex 'modulea.bicep' = [for (module, i) in myModules: {
//@    "storageResourcesWithIndex": {
//@      "copy": {
//@        "name": "storageResourcesWithIndex",
//@        "count": "[length(variables('myModules'))]"
//@      },
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
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
//@        "parameters": {
//@        },
    arrayParam: [
//@          "arrayParam": {
//@            "value": [
//@            ]
//@          },
      i + 1
//@              "[add(copyIndex(), 1)]"
    ]
    objParam: module
//@          "objParam": {
//@            "value": "[variables('myModules')[copyIndex()]]"
//@          },
    stringParamB: module.location
//@          "stringParamB": {
//@            "value": "[variables('myModules')[copyIndex()].location]"
//@          },
    stringParamA: concat('a', i)
//@          "stringParamA": {
//@            "value": "[concat('a', copyIndex())]"
//@          }
  }
}]

// nested module loop
module nestedModuleLoop 'modulea.bicep' = [for module in myModules: {
//@    "nestedModuleLoop": {
//@      "copy": {
//@        "name": "nestedModuleLoop",
//@        "count": "[length(variables('myModules'))]"
//@      },
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
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
//@        "parameters": {
//@          "arrayParam": {
//@          },
//@        },
    arrayParam: [for i in range(0,3): concat('test-', i, '-', module.name)]
//@            "copy": [
//@              {
//@                "name": "value",
//@                "count": "[length(range(0, 3))]",
//@                "input": "[concat('test-', range(0, 3)[copyIndex('value')], '-', variables('myModules')[copyIndex()].name)]"
//@              }
//@            ]
    objParam: module
//@          "objParam": {
//@            "value": "[variables('myModules')[copyIndex()]]"
//@          },
    stringParamB: module.location
//@          "stringParamB": {
//@            "value": "[variables('myModules')[copyIndex()].location]"
//@          }
  }
}]

// duplicate identifiers across scopes are allowed (inner hides the outer)
module duplicateIdentifiersWithinLoop 'modulea.bicep' = [for x in emptyArray:{
//@    "duplicateIdentifiersWithinLoop": {
//@      "copy": {
//@        "name": "duplicateIdentifiersWithinLoop",
//@        "count": "[length(variables('emptyArray'))]"
//@      },
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
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
//@        "parameters": {
//@          "arrayParam": {
//@          }
//@        },
    objParam: {}
//@          "objParam": {
//@            "value": {}
//@          },
    stringParamA: 'test'
//@          "stringParamA": {
//@            "value": "test"
//@          },
    stringParamB: 'test'
//@          "stringParamB": {
//@            "value": "test"
//@          },
    arrayParam: [for x in emptyArray: x]
//@            "copy": [
//@              {
//@                "name": "value",
//@                "count": "[length(variables('emptyArray'))]",
//@                "input": "[variables('emptyArray')[copyIndex('value')]]"
//@              }
//@            ]
  }
}]

// duplicate identifiers across scopes are allowed (inner hides the outer)
var duplicateAcrossScopes = 'hello'
//@    "duplicateAcrossScopes": "hello",
module duplicateInGlobalAndOneLoop 'modulea.bicep' = [for duplicateAcrossScopes in []: {
//@    "duplicateInGlobalAndOneLoop": {
//@      "copy": {
//@        "name": "duplicateInGlobalAndOneLoop",
//@        "count": "[length(createArray())]"
//@      },
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
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
//@        "parameters": {
//@          "arrayParam": {
//@          }
//@        },
    objParam: {}
//@          "objParam": {
//@            "value": {}
//@          },
    stringParamA: 'test'
//@          "stringParamA": {
//@            "value": "test"
//@          },
    stringParamB: 'test'
//@          "stringParamB": {
//@            "value": "test"
//@          },
    arrayParam: [for x in emptyArray: x]
//@            "copy": [
//@              {
//@                "name": "value",
//@                "count": "[length(variables('emptyArray'))]",
//@                "input": "[variables('emptyArray')[copyIndex('value')]]"
//@              }
//@            ]
  }
}]

var someDuplicate = true
//@    "someDuplicate": true,
var otherDuplicate = false
//@    "otherDuplicate": false,
module duplicatesEverywhere 'modulea.bicep' = [for someDuplicate in []: {
//@    "duplicatesEverywhere": {
//@      "copy": {
//@        "name": "duplicatesEverywhere",
//@        "count": "[length(createArray())]"
//@      },
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
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
//@        "parameters": {
//@          "arrayParam": {
//@          }
//@        },
    objParam: {}
//@          "objParam": {
//@            "value": {}
//@          },
    stringParamB: 'test'
//@          "stringParamB": {
//@            "value": "test"
//@          },
    arrayParam: [for otherDuplicate in emptyArray: '${someDuplicate}-${otherDuplicate}']
//@            "copy": [
//@              {
//@                "name": "value",
//@                "count": "[length(variables('emptyArray'))]",
//@                "input": "[format('{0}-{1}', createArray()[copyIndex()], variables('emptyArray')[copyIndex('value')])]"
//@              }
//@            ]
  }
}]

module propertyLoopInsideParameterValue 'modulea.bicep' = {
//@    "propertyLoopInsideParameterValue": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
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
//@        "parameters": {
//@        },
    objParam: {
//@          "objParam": {
//@            "value": {
//@              "copy": [
//@              ],
//@            }
//@          },
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
//@          "stringParamB": {
//@            "value": ""
//@          },
    arrayParam: [
//@          "arrayParam": {
//@            "value": [
//@            ]
//@          }
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
//@    "propertyLoopInsideParameterValueWithIndexes": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
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
//@        "parameters": {
//@        },
    objParam: {
//@          "objParam": {
//@            "value": {
//@              "copy": [
//@              ],
//@            }
//@          },
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
//@          "stringParamB": {
//@            "value": ""
//@          },
    arrayParam: [
//@          "arrayParam": {
//@            "value": [
//@            ]
//@          }
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
//@    "propertyLoopInsideParameterValueInsideModuleLoop": {
//@      "copy": {
//@        "name": "propertyLoopInsideParameterValueInsideModuleLoop",
//@        "count": "[length(range(0, 1))]"
//@      },
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
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
//@        "parameters": {
//@        },
    objParam: {
//@          "objParam": {
//@            "value": {
//@              "copy": [
//@              ],
//@            }
//@          },
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
//@          "stringParamB": {
//@            "value": ""
//@          },
    arrayParam: [
//@          "arrayParam": {
//@            "value": [
//@            ]
//@          }
      {
//@              {
//@                "copy": [
//@                ]
//@              }
        e: [for j in range(7,7): j % (thing + 1)]
//@                  {
//@                    "name": "e",
//@                    "count": "[length(range(7, 7))]",
//@                    "input": "[mod(range(7, 7)[copyIndex('e')], add(range(0, 1)[copyIndex()], 1))]"
//@                  }
      }
    ]
  }
}]


// BEGIN: Key Vault Secret Reference

resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@    "kv": {
//@      "existing": true,
//@      "type": "Microsoft.KeyVault/vaults",
//@      "apiVersion": "2019-09-01",
//@      "name": "testkeyvault"
//@    },
  name: 'testkeyvault'
}

module secureModule1 'child/secureParams.bicep' = {
//@    "secureModule1": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
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
//@        "parameters": {
//@        },
    secureStringParam1: kv.getSecret('mySecret')
//@          "secureStringParam1": {
//@            "reference": {
//@              "keyVault": {
//@                "id": "[resourceId('Microsoft.KeyVault/vaults', 'testkeyvault')]"
//@              },
//@              "secretName": "mySecret"
//@            }
//@          },
    secureStringParam2: kv.getSecret('mySecret','secretVersion')
//@          "secureStringParam2": {
//@            "reference": {
//@              "keyVault": {
//@                "id": "[resourceId('Microsoft.KeyVault/vaults', 'testkeyvault')]"
//@              },
//@              "secretName": "mySecret",
//@              "secretVersion": "secretVersion"
//@            }
//@          }
  }
}

resource scopedKv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@    "scopedKv": {
//@      "existing": true,
//@      "type": "Microsoft.KeyVault/vaults",
//@      "apiVersion": "2019-09-01",
//@      "resourceGroup": "otherGroup",
//@      "name": "testkeyvault"
//@    },
  name: 'testkeyvault'
  scope: resourceGroup('otherGroup')
}

module secureModule2 'child/secureParams.bicep' = {
//@    "secureModule2": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
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
//@        "parameters": {
//@        },
    secureStringParam1: scopedKv.getSecret('mySecret')
//@          "secureStringParam1": {
//@            "reference": {
//@              "keyVault": {
//@                "id": "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'otherGroup'), 'Microsoft.KeyVault/vaults', 'testkeyvault')]"
//@              },
//@              "secretName": "mySecret"
//@            }
//@          },
    secureStringParam2: scopedKv.getSecret('mySecret','secretVersion')
//@          "secureStringParam2": {
//@            "reference": {
//@              "keyVault": {
//@                "id": "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'otherGroup'), 'Microsoft.KeyVault/vaults', 'testkeyvault')]"
//@              },
//@              "secretName": "mySecret",
//@              "secretVersion": "secretVersion"
//@            }
//@          }
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
//@    ],
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
//@    "loopedKv": {
//@      "copy": {
//@        "name": "loopedKv",
//@        "count": "[length(variables('vaults'))]"
//@      },
//@      "existing": true,
//@      "type": "Microsoft.KeyVault/vaults",
//@      "apiVersion": "2019-09-01",
//@      "subscriptionId": "[variables('vaults')[copyIndex()].vaultSub]",
//@      "resourceGroup": "[variables('vaults')[copyIndex()].vaultRG]",
//@      "name": "[variables('vaults')[copyIndex()].vaultName]"
//@    },
  name: vault.vaultName
  scope: resourceGroup(vault.vaultSub, vault.vaultRG)
}]

module secureModuleLooped 'child/secureParams.bicep' = [for (secret, i) in secrets: {
//@    "secureModuleLooped": {
//@      "copy": {
//@        "name": "secureModuleLooped",
//@        "count": "[length(variables('secrets'))]"
//@      },
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
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
//@        "parameters": {
//@        },
    secureStringParam1: loopedKv[i].getSecret(secret.name)
//@          "secureStringParam1": {
//@            "reference": {
//@              "keyVault": {
//@                "id": "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', variables('vaults')[copyIndex()].vaultSub, variables('vaults')[copyIndex()].vaultRG), 'Microsoft.KeyVault/vaults', variables('vaults')[copyIndex()].vaultName)]"
//@              },
//@              "secretName": "[variables('secrets')[copyIndex()].name]"
//@            }
//@          },
    secureStringParam2: loopedKv[i].getSecret(secret.name, secret.version)
//@          "secureStringParam2": {
//@            "reference": {
//@              "keyVault": {
//@                "id": "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', variables('vaults')[copyIndex()].vaultSub, variables('vaults')[copyIndex()].vaultRG), 'Microsoft.KeyVault/vaults', variables('vaults')[copyIndex()].vaultName)]"
//@              },
//@              "secretName": "[variables('secrets')[copyIndex()].name]",
//@              "secretVersion": "[variables('secrets')[copyIndex()].version]"
//@            }
//@          }
  }
}]

module secureModuleCondition 'child/secureParams.bicep' = {
//@    "secureModuleCondition": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
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
//@        "parameters": {
//@        },
    secureStringParam1: true ? kv.getSecret('mySecret') : 'notTrue'
//@          "secureStringParam1": "[if(true(), createObject('reference', createObject('keyVault', createObject('id', resourceId('Microsoft.KeyVault/vaults', 'testkeyvault')), 'secretName', 'mySecret')), createObject('value', 'notTrue'))]",
    secureStringParam2: true ? false ? 'false' : kv.getSecret('mySecret','secretVersion') : 'notTrue'
//@          "secureStringParam2": "[if(true(), if(false(), createObject('value', 'false'), createObject('reference', createObject('keyVault', createObject('id', resourceId('Microsoft.KeyVault/vaults', 'testkeyvault')), 'secretName', 'mySecret', 'secretVersion', 'secretVersion'))), createObject('value', 'notTrue'))]"
  }
}

// END: Key Vault Secret Reference

module withSpace 'module with space.bicep' = {
//@    "withSpace": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
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
//@    "folderWithSpace": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
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

// nameof

var nameofModule = nameof(folderWithSpace)
//@    "nameofModule": "folderWithSpace",
var nameofModuleParam = nameof(secureModuleCondition.outputs.exposedSecureString)
//@    "nameofModuleParam": "exposedSecureString"

module moduleWithNameof 'modulea.bicep' = {
//@    "moduleWithNameof": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "resourceGroup": "nameofModuleParam",
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
//@      "dependsOn": [
//@        "folderWithSpace",
//@        "secureModuleCondition",
//@        "withSpace"
//@      ]
//@    },
  name: 'nameofModule'
//@      "name": "nameofModule",
  scope: resourceGroup(nameof(nameofModuleParam))
  params:{
//@        "parameters": {
//@        },
    stringParamA: nameof(withSpace)
//@          "stringParamA": {
//@            "value": "withSpace"
//@          },
    stringParamB: nameof(folderWithSpace)
//@          "stringParamB": {
//@            "value": "folderWithSpace"
//@          },
    objParam: {
//@          "objParam": {
//@            "value": {
//@            }
//@          },
      a: nameof(secureModuleCondition.outputs.exposedSecureString)
//@              "a": "exposedSecureString"
    }
    arrayParam: [
//@          "arrayParam": {
//@            "value": [
//@            ]
//@          }
      nameof(vaults)
//@              "vaults"
    ]
  }
}

module moduleWithNullableOutputs 'child/nullableOutputs.bicep' = {
//@    "moduleWithNullableOutputs": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "languageVersion": "2.0",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "9938797971770597442"
//@            }
//@          },
//@          "resources": {},
//@          "outputs": {
//@            "nullableString": {
//@              "type": "string",
//@              "nullable": true,
//@              "value": "foo"
//@            },
//@            "nullableObj": {
//@              "type": "object",
//@              "nullable": true,
//@              "value": {
//@                "deeply": {
//@                  "nested": {
//@                    "property": "value",
//@                    "array": [
//@                      "foo",
//@                      "bar"
//@                    ]
//@                  }
//@                }
//@              }
//@            }
//@          }
//@        }
//@      }
//@    }
  name: 'nullableOutputs'
//@      "name": "nullableOutputs",
}

output nullableString string? = moduleWithNullableOutputs.outputs.?nullableString
//@    "nullableString": {
//@      "type": "string",
//@      "nullable": true,
//@      "value": "[tryGet(tryGet(reference('moduleWithNullableOutputs').outputs, 'nullableString'), 'value')]"
//@    },
output deeplyNestedProperty string? = moduleWithNullableOutputs.outputs.?nullableObj.deeply.nested.property
//@    "deeplyNestedProperty": {
//@      "type": "string",
//@      "nullable": true,
//@      "value": "[tryGet(tryGet(reference('moduleWithNullableOutputs').outputs, 'nullableObj'), 'value', 'deeply', 'nested', 'property')]"
//@    },
output deeplyNestedArrayItem string? = moduleWithNullableOutputs.outputs.?nullableObj.deeply.nested.array[0]
//@    "deeplyNestedArrayItem": {
//@      "type": "string",
//@      "nullable": true,
//@      "value": "[tryGet(tryGet(reference('moduleWithNullableOutputs').outputs, 'nullableObj'), 'value', 'deeply', 'nested', 'array', 0)]"
//@    },
output deeplyNestedArrayItemFromEnd string? = moduleWithNullableOutputs.outputs.?nullableObj.deeply.nested.array[^1]
//@    "deeplyNestedArrayItemFromEnd": {
//@      "type": "string",
//@      "nullable": true,
//@      "value": "[tryGet(tryGet(reference('moduleWithNullableOutputs').outputs, 'nullableObj'), 'value', 'deeply', 'nested', 'array', createObject('value', 1, 'fromEnd', true()))]"
//@    },
output deeplyNestedArrayItemFromEndAttempt string? = moduleWithNullableOutputs.outputs.?nullableObj.deeply.nested.array[?^1]
//@    "deeplyNestedArrayItemFromEndAttempt": {
//@      "type": "string",
//@      "nullable": true,
//@      "value": "[tryIndexFromEnd(tryGet(tryGet(reference('moduleWithNullableOutputs').outputs, 'nullableObj'), 'value', 'deeply', 'nested', 'array'), 1)]"
//@    }

