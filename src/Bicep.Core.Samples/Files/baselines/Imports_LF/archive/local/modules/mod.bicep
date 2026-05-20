{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "languageVersion": "2.0",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "5837947923069569751"
    },
    "__bicep_exported_variables!": [
      {
        "name": "foo"
      }
    ]
  },
  "definitions": {
    "fizz": {
      "type": "array",
      "items": {
        "$ref": "#/definitions/buzz"
      },
      "metadata": {
        "__bicep_export!": true
      }
    },
    "buzz": {
      "type": "object",
      "properties": {
        "property": {
          "$ref": "#/definitions/pop",
          "nullable": true
        }
      }
    },
    "pop": {
      "type": "string",
      "metadata": {
        "__bicep_export!": true
      },
      "minLength": 3
    }
  },
  "functions": [
    {
      "namespace": "__bicep",
      "members": {
        "echo": {
          "parameters": [
            {
              "type": "string",
              "name": "input"
            }
          ],
          "output": {
            "type": "string",
            "value": "[parameters('input')]"
          }
        },
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
            "__bicep_export!": true
          }
        }
      }
    }
  ],
  "variables": {
    "foo": "[variables('bar')]",
    "bar": "[variables('baz')]",
    "baz": "quux"
  },
  "resources": {}
}