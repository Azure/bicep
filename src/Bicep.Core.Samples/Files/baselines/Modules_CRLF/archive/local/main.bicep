{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "languageVersion": "2.0",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "4349180616532777767"
    }
  },
  "parameters": {
    "deployTimeSuffix": {
      "type": "string",
      "defaultValue": "[newGuid()]",
      "metadata": {
        "description": "this is deployTimeSuffix param"
      }
    }
  },
  "variables": {
    "myModules": [
      {
        "name": "one",
        "location": "eastus2"
      },
      {
        "name": "two",
        "location": "westus"
      }
    ],
    "emptyArray": [],
    "duplicateAcrossScopes": "hello",
    "someDuplicate": true,
    "otherDuplicate": false,
    "vaults": [
      {
        "vaultName": "test-1-kv",
        "vaultRG": "test-1-rg",
        "vaultSub": "abcd-efgh"
      },
      {
        "vaultName": "test-2-kv",
        "vaultRG": "test-2-rg",
        "vaultSub": "ijkl-1adg1"
      }
    ],
    "secrets": [
      {
        "name": "secret01",
        "version": "versionA"
      },
      {
        "name": "secret02",
        "version": "versionB"
      }
    ],
    "nameofModule": "folderWithSpace",
    "nameofModuleParam": "exposedSecureString"
  },
  "resources": {
    "resWithDependencies": {
      "type": "Mock.Rp/mockResource",
      "apiVersion": "2020-01-01",
      "name": "harry",
      "properties": {
        "modADep": "[reference('modATest').outputs.stringOutputA.value]",
        "modBDep": "[reference('modB').outputs.myResourceId.value]",
        "modCDep": "[reference('modC').outputs.myResourceId.value]"
      },
      "dependsOn": [
        "modATest",
        "modB",
        "modC"
      ]
    },
    "resWithCalculatedNameDependencies": {
      "type": "Mock.Rp/mockResource",
      "apiVersion": "2020-01-01",
      "name": "[format('{0}{1}', 'optionalWithAllParamsAndManualDependency', parameters('deployTimeSuffix'))]",
      "properties": {
        "modADep": "[reference('moduleWithCalculatedName').outputs.outputObj.value]"
      },
      "dependsOn": [
        "moduleWithCalculatedName",
        "optionalWithAllParamsAndManualDependency"
      ]
    },
    "kv": {
      "existing": true,
      "type": "Microsoft.KeyVault/vaults",
      "apiVersion": "2019-09-01",
      "name": "testkeyvault"
    },
    "scopedKv": {
      "existing": true,
      "type": "Microsoft.KeyVault/vaults",
      "apiVersion": "2019-09-01",
      "resourceGroup": "otherGroup",
      "name": "testkeyvault"
    },
    "loopedKv": {
      "copy": {
        "name": "loopedKv",
        "count": "[length(variables('vaults'))]"
      },
      "existing": true,
      "type": "Microsoft.KeyVault/vaults",
      "apiVersion": "2019-09-01",
      "subscriptionId": "[variables('vaults')[copyIndex()].vaultSub]",
      "resourceGroup": "[variables('vaults')[copyIndex()].vaultRG]",
      "name": "[variables('vaults')[copyIndex()].vaultName]"
    },
    "modATest": {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "modATest",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "stringParamB": {
            "value": "hello!"
          },
          "objParam": {
            "value": {
              "a": "b"
            }
          },
          "arrayParam": {
            "value": [
              {
                "a": "b"
              },
              "abc"
            ]
          }
        },
        "templateLink": {
          "relativePath": "modulea.bicep"
        }
      },
      "metadata": {
        "description": "this module a"
      }
    },
    "modB": {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "modB",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "location": {
            "value": "West US"
          }
        },
        "templateLink": {
          "relativePath": "child/moduleb.bicep"
        }
      },
      "metadata": {
        "description": "this module b"
      }
    },
    "modBWithCondition": {
      "condition": "[equals(add(1, 1), 2)]",
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "modBWithCondition",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "location": {
            "value": "East US"
          }
        },
        "templateLink": {
          "relativePath": "child/moduleb.bicep"
        }
      },
      "metadata": {
        "description": "this is just module b with a condition"
      }
    },
    "modBWithCondition2": {
      "condition": "[equals(add(1, 1), 2)]",
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "modBWithCondition2",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "location": {
            "value": "East US"
          }
        },
        "templateLink": {
          "relativePath": "child/moduleb.bicep"
        }
      }
    },
    "modC": {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "modC",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "location": {
            "value": "West US"
          }
        },
        "templateLink": {
          "relativePath": "child/modulec.json"
        }
      }
    },
    "modCWithCondition": {
      "condition": "[equals(sub(2, 1), 1)]",
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "modCWithCondition",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "location": {
            "value": "East US"
          }
        },
        "templateLink": {
          "relativePath": "child/modulec.json"
        }
      }
    },
    "optionalWithNoParams1": {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "optionalWithNoParams1",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "templateLink": {
          "relativePath": "child/optionalParams.bicep"
        }
      }
    },
    "optionalWithNoParams2": {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "optionalWithNoParams2",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {},
        "templateLink": {
          "relativePath": "child/optionalParams.bicep"
        }
      }
    },
    "optionalWithAllParams": {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "optionalWithNoParams3",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "optionalString": {
            "value": "abc"
          },
          "optionalInt": {
            "value": 42
          },
          "optionalObj": {
            "value": {}
          },
          "optionalArray": {
            "value": []
          }
        },
        "templateLink": {
          "relativePath": "child/optionalParams.bicep"
        }
      }
    },
    "optionalWithAllParamsAndManualDependency": {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "optionalWithAllParamsAndManualDependency",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "optionalString": {
            "value": "abc"
          },
          "optionalInt": {
            "value": 42
          },
          "optionalObj": {
            "value": {}
          },
          "optionalArray": {
            "value": []
          }
        },
        "templateLink": {
          "relativePath": "child/optionalParams.bicep"
        }
      },
      "dependsOn": [
        "optionalWithAllParams",
        "resWithDependencies"
      ]
    },
    "optionalWithImplicitDependency": {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "optionalWithImplicitDependency",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "optionalString": {
            "value": "[concat(resourceId('Mock.Rp/mockResource', 'harry'), 'optionalWithAllParamsAndManualDependency')]"
          },
          "optionalInt": {
            "value": 42
          },
          "optionalObj": {
            "value": {}
          },
          "optionalArray": {
            "value": []
          }
        },
        "templateLink": {
          "relativePath": "child/optionalParams.bicep"
        }
      },
      "dependsOn": [
        "optionalWithAllParamsAndManualDependency",
        "resWithDependencies"
      ]
    },
    "moduleWithCalculatedName": {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "[format('{0}{1}', 'optionalWithAllParamsAndManualDependency', parameters('deployTimeSuffix'))]",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "optionalString": {
            "value": "[concat(resourceId('Mock.Rp/mockResource', 'harry'), 'optionalWithAllParamsAndManualDependency')]"
          },
          "optionalInt": {
            "value": 42
          },
          "optionalObj": {
            "value": {}
          },
          "optionalArray": {
            "value": []
          }
        },
        "templateLink": {
          "relativePath": "child/optionalParams.bicep"
        }
      },
      "dependsOn": [
        "optionalWithAllParamsAndManualDependency",
        "resWithDependencies"
      ]
    },
    "storageResources": {
      "copy": {
        "name": "storageResources",
        "count": "[length(variables('myModules'))]"
      },
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "[variables('myModules')[copyIndex()].name]",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "arrayParam": {
            "value": []
          },
          "objParam": {
            "value": "[variables('myModules')[copyIndex()]]"
          },
          "stringParamB": {
            "value": "[variables('myModules')[copyIndex()].location]"
          }
        },
        "templateLink": {
          "relativePath": "modulea.bicep"
        }
      }
    },
    "storageResourcesWithIndex": {
      "copy": {
        "name": "storageResourcesWithIndex",
        "count": "[length(variables('myModules'))]"
      },
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "[variables('myModules')[copyIndex()].name]",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "arrayParam": {
            "value": [
              "[add(copyIndex(), 1)]"
            ]
          },
          "objParam": {
            "value": "[variables('myModules')[copyIndex()]]"
          },
          "stringParamB": {
            "value": "[variables('myModules')[copyIndex()].location]"
          },
          "stringParamA": {
            "value": "[concat('a', copyIndex())]"
          }
        },
        "templateLink": {
          "relativePath": "modulea.bicep"
        }
      }
    },
    "nestedModuleLoop": {
      "copy": {
        "name": "nestedModuleLoop",
        "count": "[length(variables('myModules'))]"
      },
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "[variables('myModules')[copyIndex()].name]",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "arrayParam": {
            "copy": [
              {
                "name": "value",
                "count": "[length(range(0, 3))]",
                "input": "[concat('test-', range(0, 3)[copyIndex('value')], '-', variables('myModules')[copyIndex()].name)]"
              }
            ]
          },
          "objParam": {
            "value": "[variables('myModules')[copyIndex()]]"
          },
          "stringParamB": {
            "value": "[variables('myModules')[copyIndex()].location]"
          }
        },
        "templateLink": {
          "relativePath": "modulea.bicep"
        }
      }
    },
    "duplicateIdentifiersWithinLoop": {
      "copy": {
        "name": "duplicateIdentifiersWithinLoop",
        "count": "[length(variables('emptyArray'))]"
      },
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "[format('hello-{0}', variables('emptyArray')[copyIndex()])]",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "objParam": {
            "value": {}
          },
          "stringParamA": {
            "value": "test"
          },
          "stringParamB": {
            "value": "test"
          },
          "arrayParam": {
            "copy": [
              {
                "name": "value",
                "count": "[length(variables('emptyArray'))]",
                "input": "[variables('emptyArray')[copyIndex('value')]]"
              }
            ]
          }
        },
        "templateLink": {
          "relativePath": "modulea.bicep"
        }
      }
    },
    "duplicateInGlobalAndOneLoop": {
      "copy": {
        "name": "duplicateInGlobalAndOneLoop",
        "count": "[length(createArray())]"
      },
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "[format('hello-{0}', createArray()[copyIndex()])]",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "objParam": {
            "value": {}
          },
          "stringParamA": {
            "value": "test"
          },
          "stringParamB": {
            "value": "test"
          },
          "arrayParam": {
            "copy": [
              {
                "name": "value",
                "count": "[length(variables('emptyArray'))]",
                "input": "[variables('emptyArray')[copyIndex('value')]]"
              }
            ]
          }
        },
        "templateLink": {
          "relativePath": "modulea.bicep"
        }
      }
    },
    "duplicatesEverywhere": {
      "copy": {
        "name": "duplicatesEverywhere",
        "count": "[length(createArray())]"
      },
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "[format('hello-{0}', createArray()[copyIndex()])]",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "objParam": {
            "value": {}
          },
          "stringParamB": {
            "value": "test"
          },
          "arrayParam": {
            "copy": [
              {
                "name": "value",
                "count": "[length(variables('emptyArray'))]",
                "input": "[format('{0}-{1}', createArray()[copyIndex()], variables('emptyArray')[copyIndex('value')])]"
              }
            ]
          }
        },
        "templateLink": {
          "relativePath": "modulea.bicep"
        }
      }
    },
    "propertyLoopInsideParameterValue": {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "propertyLoopInsideParameterValue",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "objParam": {
            "value": {
              "copy": [
                {
                  "name": "a",
                  "count": "[length(range(0, 10))]",
                  "input": "[range(0, 10)[copyIndex('a')]]"
                },
                {
                  "name": "b",
                  "count": "[length(range(1, 2))]",
                  "input": "[range(1, 2)[copyIndex('b')]]"
                },
                {
                  "name": "e",
                  "count": "[length(range(4, 4))]",
                  "input": {
                    "f": "[range(4, 4)[copyIndex('e')]]"
                  }
                }
              ],
              "c": {
                "copy": [
                  {
                    "name": "d",
                    "count": "[length(range(2, 3))]",
                    "input": "[range(2, 3)[copyIndex('d')]]"
                  }
                ]
              }
            }
          },
          "stringParamB": {
            "value": ""
          },
          "arrayParam": {
            "value": [
              {
                "copy": [
                  {
                    "name": "e",
                    "count": "[length(range(7, 7))]",
                    "input": "[range(7, 7)[copyIndex('e')]]"
                  }
                ]
              }
            ]
          }
        },
        "templateLink": {
          "relativePath": "modulea.bicep"
        }
      }
    },
    "propertyLoopInsideParameterValueWithIndexes": {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "propertyLoopInsideParameterValueWithIndexes",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "objParam": {
            "value": {
              "copy": [
                {
                  "name": "a",
                  "count": "[length(range(0, 10))]",
                  "input": "[add(range(0, 10)[copyIndex('a')], copyIndex('a'))]"
                },
                {
                  "name": "b",
                  "count": "[length(range(1, 2))]",
                  "input": "[div(range(1, 2)[copyIndex('b')], copyIndex('b'))]"
                },
                {
                  "name": "e",
                  "count": "[length(range(4, 4))]",
                  "input": {
                    "f": "[range(4, 4)[copyIndex('e')]]",
                    "g": "[copyIndex('e')]"
                  }
                }
              ],
              "c": {
                "copy": [
                  {
                    "name": "d",
                    "count": "[length(range(2, 3))]",
                    "input": "[mul(range(2, 3)[copyIndex('d')], copyIndex('d'))]"
                  }
                ]
              }
            }
          },
          "stringParamB": {
            "value": ""
          },
          "arrayParam": {
            "value": [
              {
                "copy": [
                  {
                    "name": "e",
                    "count": "[length(range(7, 7))]",
                    "input": "[range(7, 7)[copyIndex('e')]]"
                  }
                ]
              }
            ]
          }
        },
        "templateLink": {
          "relativePath": "modulea.bicep"
        }
      }
    },
    "propertyLoopInsideParameterValueInsideModuleLoop": {
      "copy": {
        "name": "propertyLoopInsideParameterValueInsideModuleLoop",
        "count": "[length(range(0, 1))]"
      },
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "propertyLoopInsideParameterValueInsideModuleLoop",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "objParam": {
            "value": {
              "copy": [
                {
                  "name": "a",
                  "count": "[length(range(0, 10))]",
                  "input": "[add(range(0, 10)[copyIndex('a')], range(0, 1)[copyIndex()])]"
                },
                {
                  "name": "b",
                  "count": "[length(range(1, 2))]",
                  "input": "[mul(range(1, 2)[copyIndex('b')], range(0, 1)[copyIndex()])]"
                },
                {
                  "name": "e",
                  "count": "[length(range(4, 4))]",
                  "input": {
                    "f": "[sub(range(4, 4)[copyIndex('e')], range(0, 1)[copyIndex()])]"
                  }
                }
              ],
              "c": {
                "copy": [
                  {
                    "name": "d",
                    "count": "[length(range(2, 3))]",
                    "input": "[range(2, 3)[copyIndex('d')]]"
                  }
                ]
              }
            }
          },
          "stringParamB": {
            "value": ""
          },
          "arrayParam": {
            "value": [
              {
                "copy": [
                  {
                    "name": "e",
                    "count": "[length(range(7, 7))]",
                    "input": "[mod(range(7, 7)[copyIndex('e')], add(range(0, 1)[copyIndex()], 1))]"
                  }
                ]
              }
            ]
          }
        },
        "templateLink": {
          "relativePath": "modulea.bicep"
        }
      }
    },
    "secureModule1": {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "secureModule1",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "secureStringParam1": {
            "reference": {
              "keyVault": {
                "id": "[resourceId('Microsoft.KeyVault/vaults', 'testkeyvault')]"
              },
              "secretName": "mySecret"
            }
          },
          "secureStringParam2": {
            "reference": {
              "keyVault": {
                "id": "[resourceId('Microsoft.KeyVault/vaults', 'testkeyvault')]"
              },
              "secretName": "mySecret",
              "secretVersion": "secretVersion"
            }
          }
        },
        "templateLink": {
          "relativePath": "child/secureParams.bicep"
        }
      }
    },
    "secureModule2": {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "secureModule2",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "secureStringParam1": {
            "reference": {
              "keyVault": {
                "id": "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'otherGroup'), 'Microsoft.KeyVault/vaults', 'testkeyvault')]"
              },
              "secretName": "mySecret"
            }
          },
          "secureStringParam2": {
            "reference": {
              "keyVault": {
                "id": "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'otherGroup'), 'Microsoft.KeyVault/vaults', 'testkeyvault')]"
              },
              "secretName": "mySecret",
              "secretVersion": "secretVersion"
            }
          }
        },
        "templateLink": {
          "relativePath": "child/secureParams.bicep"
        }
      }
    },
    "secureModuleLooped": {
      "copy": {
        "name": "secureModuleLooped",
        "count": "[length(variables('secrets'))]"
      },
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "[format('secureModuleLooped-{0}', copyIndex())]",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "secureStringParam1": {
            "reference": {
              "keyVault": {
                "id": "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', variables('vaults')[copyIndex()].vaultSub, variables('vaults')[copyIndex()].vaultRG), 'Microsoft.KeyVault/vaults', variables('vaults')[copyIndex()].vaultName)]"
              },
              "secretName": "[variables('secrets')[copyIndex()].name]"
            }
          },
          "secureStringParam2": {
            "reference": {
              "keyVault": {
                "id": "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', variables('vaults')[copyIndex()].vaultSub, variables('vaults')[copyIndex()].vaultRG), 'Microsoft.KeyVault/vaults', variables('vaults')[copyIndex()].vaultName)]"
              },
              "secretName": "[variables('secrets')[copyIndex()].name]",
              "secretVersion": "[variables('secrets')[copyIndex()].version]"
            }
          }
        },
        "templateLink": {
          "relativePath": "child/secureParams.bicep"
        }
      }
    },
    "secureModuleCondition": {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "secureModuleCondition",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "secureStringParam1": "[if(true(), createObject('reference', createObject('keyVault', createObject('id', resourceId('Microsoft.KeyVault/vaults', 'testkeyvault')), 'secretName', 'mySecret')), createObject('value', 'notTrue'))]",
          "secureStringParam2": "[if(true(), if(false(), createObject('value', 'false'), createObject('reference', createObject('keyVault', createObject('id', resourceId('Microsoft.KeyVault/vaults', 'testkeyvault')), 'secretName', 'mySecret', 'secretVersion', 'secretVersion'))), createObject('value', 'notTrue'))]"
        },
        "templateLink": {
          "relativePath": "child/secureParams.bicep"
        }
      }
    },
    "withSpace": {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "withSpace",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "templateLink": {
          "relativePath": "module with space.bicep"
        }
      }
    },
    "folderWithSpace": {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "childWithSpace",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "templateLink": {
          "relativePath": "child/folder with space/child with space.bicep"
        }
      }
    },
    "moduleWithNameof": {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "nameofModule",
      "resourceGroup": "nameofModuleParam",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "stringParamA": {
            "value": "withSpace"
          },
          "stringParamB": {
            "value": "folderWithSpace"
          },
          "objParam": {
            "value": {
              "a": "exposedSecureString"
            }
          },
          "arrayParam": {
            "value": [
              "vaults"
            ]
          }
        },
        "templateLink": {
          "relativePath": "modulea.bicep"
        }
      },
      "dependsOn": [
        "folderWithSpace",
        "secureModuleCondition",
        "withSpace"
      ]
    },
    "moduleWithNullableOutputs": {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "nullableOutputs",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "templateLink": {
          "relativePath": "child/nullableOutputs.bicep"
        }
      }
    }
  },
  "outputs": {
    "stringOutputA": {
      "type": "string",
      "value": "[reference('modATest').outputs.stringOutputA.value]"
    },
    "stringOutputB": {
      "type": "string",
      "value": "[reference('modATest').outputs.stringOutputB.value]"
    },
    "objOutput": {
      "type": "object",
      "value": "[reference('modATest').outputs.objOutput.value]"
    },
    "arrayOutput": {
      "type": "array",
      "value": "[reference('modATest').outputs.arrayOutput.value]"
    },
    "modCalculatedNameOutput": {
      "type": "object",
      "value": "[reference('moduleWithCalculatedName').outputs.outputObj.value]"
    },
    "nullableString": {
      "type": "string",
      "nullable": true,
      "value": "[tryGet(tryGet(reference('moduleWithNullableOutputs').outputs, 'nullableString'), 'value')]"
    },
    "deeplyNestedProperty": {
      "type": "string",
      "nullable": true,
      "value": "[tryGet(tryGet(reference('moduleWithNullableOutputs').outputs, 'nullableObj'), 'value', 'deeply', 'nested', 'property')]"
    },
    "deeplyNestedArrayItem": {
      "type": "string",
      "nullable": true,
      "value": "[tryGet(tryGet(reference('moduleWithNullableOutputs').outputs, 'nullableObj'), 'value', 'deeply', 'nested', 'array', 0)]"
    },
    "deeplyNestedArrayItemFromEnd": {
      "type": "string",
      "nullable": true,
      "value": "[tryGet(tryGet(reference('moduleWithNullableOutputs').outputs, 'nullableObj'), 'value', 'deeply', 'nested', 'array', createObject('value', 1, 'fromEnd', true()))]"
    },
    "deeplyNestedArrayItemFromEndAttempt": {
      "type": "string",
      "nullable": true,
      "value": "[tryIndexFromEnd(tryGet(tryGet(reference('moduleWithNullableOutputs').outputs, 'nullableObj'), 'value', 'deeply', 'nested', 'array'), 1)]"
    }
  }
}