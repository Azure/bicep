{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "languageVersion": "2.0",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "2216132208843131284"
    }
  },
  "definitions": {
    "positiveInt": {
      "type": "int",
      "minValue": 0
    }
  },
  "functions": [
    {
      "namespace": "__bicep",
      "members": {
        "buildUrl": {
          "parameters": [
            {
              "type": "bool",
              "name": "https"
            },
            {
              "type": "string",
              "name": "hostname"
            },
            {
              "type": "string",
              "name": "path"
            }
          ],
          "output": {
            "type": "string",
            "value": "[format('{0}://{1}{2}', if(parameters('https'), 'https', 'http'), parameters('hostname'), if(empty(parameters('path')), '', format('/{0}', parameters('path'))))]"
          }
        },
        "sayHello": {
          "parameters": [
            {
              "type": "string",
              "name": "name"
            }
          ],
          "output": {
            "type": "string",
            "value": "[format('Hi {0}!', parameters('name'))]"
          }
        },
        "objReturnType": {
          "parameters": [
            {
              "type": "string",
              "name": "name"
            }
          ],
          "output": {
            "type": "object",
            "value": {
              "hello": "[format('Hi {0}!', parameters('name'))]"
            }
          }
        },
        "arrayReturnType": {
          "parameters": [
            {
              "type": "string",
              "name": "name"
            }
          ],
          "output": {
            "type": "array",
            "value": [
              "[parameters('name')]"
            ]
          }
        },
        "asdf": {
          "parameters": [
            {
              "type": "string",
              "name": "name"
            }
          ],
          "output": {
            "type": "array",
            "value": [
              "asdf",
              "[parameters('name')]"
            ]
          }
        },
        "typedArg": {
          "parameters": [
            {
              "type": "array",
              "items": {
                "type": "string"
              },
              "name": "input"
            }
          ],
          "output": {
            "$ref": "#/definitions/positiveInt",
            "value": "[length(parameters('input'))]"
          }
        },
        "barTest": {
          "parameters": [],
          "output": {
            "type": "array",
            "value": [
              "abc",
              "def"
            ]
          }
        },
        "fooTest": {
          "parameters": [],
          "output": {
            "type": "array",
            "value": "[map(__bicep.barTest(), lambda('a', format('Hello {0}!', lambdaVariables('a'))))]"
          }
        },
        "test": {
          "parameters": [],
          "output": {
            "type": "object",
            "value": {}
          }
        },
        "test2": {
          "parameters": [],
          "output": {
            "type": "string",
            "value": "{}"
          }
        },
        "test3": {
          "parameters": [],
          "output": {
            "type": "object",
            "value": {}
          }
        },
        "test4": {
          "parameters": [],
          "output": {
            "type": "string",
            "value": "e30="
          }
        },
        "a": {
          "parameters": [
            {
              "type": "string",
              "name": "____________________________________________________________________________________________"
            }
          ],
          "output": {
            "type": "string",
            "value": "a"
          }
        },
        "b": {
          "parameters": [
            {
              "type": "string",
              "name": "longParameterName1"
            },
            {
              "type": "string",
              "name": "longParameterName2"
            },
            {
              "type": "string",
              "name": "longParameterName3"
            },
            {
              "type": "string",
              "name": "longParameterName4"
            }
          ],
          "output": {
            "type": "string",
            "value": "b"
          }
        },
        "buildUrlMultiLine": {
          "parameters": [
            {
              "type": "bool",
              "name": "https"
            },
            {
              "type": "string",
              "name": "hostname"
            },
            {
              "type": "string",
              "name": "path"
            }
          ],
          "output": {
            "type": "string",
            "value": "[format('{0}://{1}{2}', if(parameters('https'), 'https', 'http'), parameters('hostname'), if(empty(parameters('path')), '', format('/{0}', parameters('path'))))]"
          }
        }
      }
    }
  ],
  "resources": {},
  "outputs": {
    "foo": {
      "type": "string",
      "value": "[__bicep.buildUrl(true(), 'google.com', 'search')]"
    },
    "hellos": {
      "type": "array",
      "value": "[map(createArray('Evie', 'Casper'), lambda('name', __bicep.sayHello(lambdaVariables('name'))))]"
    },
    "fooValue": {
      "type": "array",
      "value": "[__bicep.fooTest()]"
    }
  }
}