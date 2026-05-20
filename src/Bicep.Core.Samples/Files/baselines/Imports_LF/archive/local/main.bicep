{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "languageVersion": "2.0",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "18183633158511667674"
    }
  },
  "definitions": {
    "fizzes": {
      "type": "array",
      "items": {
        "$ref": "#/definitions/fizz"
      }
    },
    "_1.buzz": {
      "type": "object",
      "properties": {
        "property": {
          "$ref": "#/definitions/pop",
          "nullable": true
        }
      },
      "metadata": {
        "__bicep_imported_from!": {
          "sourceTemplate": "modules/mod.bicep"
        }
      }
    },
    "_3.fizz": {
      "type": "string",
      "allowedValues": [
        "buzz"
      ],
      "metadata": {
        "__bicep_imported_from!": {
          "sourceTemplate": "modules/mod2.bicep"
        }
      }
    },
    "fizz": {
      "type": "array",
      "items": {
        "$ref": "#/definitions/_1.buzz"
      },
      "metadata": {
        "__bicep_imported_from!": {
          "sourceTemplate": "modules/mod.bicep"
        }
      }
    },
    "pop": {
      "type": "string",
      "minLength": 3,
      "metadata": {
        "__bicep_imported_from!": {
          "sourceTemplate": "modules/mod.bicep"
        }
      }
    }
  },
  "functions": [
    {
      "namespace": "__bicep",
      "members": {
        "greet": {
          "parameters": [
            {
              "type": "string",
              "name": "name"
            }
          ],
          "output": {
            "type": "string",
            "value": "[format('Hi, {0}!', parameters('name'))]"
          },
          "metadata": {
            "description": "Say hi to someone!",
            "__bicep_imported_from!": {
              "sourceTemplate": "modules/mod.bicep"
            }
          }
        }
      }
    }
  ],
  "parameters": {
    "fizzParam": {
      "$ref": "#/definitions/_3.fizz"
    }
  },
  "variables": {
    "copy": [
      {
        "name": "_2.copyVariable",
        "count": "[length(range(0, variables('_2.copyVariableCount')))]",
        "input": {
          "count": "[copyIndex('_2.copyVariable')]",
          "value": "[variables('_2.notExported')[mod(copyIndex('_2.copyVariable'), 2)]]"
        }
      }
    ],
    "aliasedFoo": "[variables('foo')]",
    "aliasedBar": "[variables('_3.foo')]",
    "_1.bar": "[variables('_1.baz')]",
    "_1.baz": "quux",
    "_2.copyVariableCount": 10,
    "_2.notExported": [
      "bippity boppity boop",
      "abracadabra"
    ],
    "_3.foo": "bar",
    "foo": "[variables('_1.bar')]",
    "refersToCopyVariable": "[variables('_2.copyVariable')]",
    "withInvalidIdentifier": "value"
  },
  "resources": {},
  "outputs": {
    "magicWord": {
      "$ref": "#/definitions/pop",
      "value": "[variables('refersToCopyVariable')[3].value]"
    },
    "greeting": {
      "type": "string",
      "value": "[__bicep.greet('friend')]"
    }
  }
}