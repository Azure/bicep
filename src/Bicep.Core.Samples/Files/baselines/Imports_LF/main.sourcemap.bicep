import {foo, fizz, pop, greet} from 'modules/mod.bicep'
//@    "_1.buzz": {
//@      "type": "object",
//@      "properties": {
//@        "property": {
//@          "$ref": "#/definitions/pop",
//@          "nullable": true
//@        }
//@      },
//@      "metadata": {
//@        "__bicep_imported_from!": {
//@          "sourceTemplate": "modules/mod.bicep"
//@        }
//@      }
//@    },
//@    "fizz": {
//@      "type": "array",
//@      "items": {
//@        "$ref": "#/definitions/_1.buzz"
//@      },
//@      "metadata": {
//@        "__bicep_imported_from!": {
//@          "sourceTemplate": "modules/mod.bicep"
//@        }
//@      }
//@    },
//@    "pop": {
//@      "type": "string",
//@      "metadata": {
//@        "__bicep_imported_from!": {
//@          "sourceTemplate": "modules/mod.bicep"
//@        }
//@      }
//@    }
//@        "greet": {
//@          "parameters": [
//@            {
//@              "type": "string",
//@              "name": "name"
//@            }
//@          ],
//@          "output": {
//@            "type": "string",
//@            "value": "[format('Hi, {0}!', parameters('name'))]"
//@          },
//@          "metadata": {
//@            "__bicep_imported_from!": {
//@              "sourceTemplate": "modules/mod.bicep"
//@            }
//@          }
//@        }
//@    "_1.bar": "[variables('_1.baz')]",
//@    "_1.baz": "quux",
//@    "foo": "[variables('_1.bar')]",
import * as mod2 from 'modules/mod2.bicep'
//@    "_3.fizz": {
//@      "type": "string",
//@      "allowedValues": [
//@        "buzz"
//@      ],
//@      "metadata": {
//@        "__bicep_imported_from!": {
//@          "sourceTemplate": "modules/mod2.bicep"
//@        }
//@      }
//@    },
//@    "_3.foo": "bar",
import {
  'not-a-valid-bicep-identifier' as withInvalidIdentifier
//@      "minLength": 3,
//@    "withInvalidIdentifier": "value"
  refersToCopyVariable
//@      {
//@        "name": "_2.copyVariable",
//@        "count": "[length(range(0, variables('_2.copyVariableCount')))]",
//@        "input": {
//@          "count": "[copyIndex('_2.copyVariable')]",
//@          "value": "[variables('_2.notExported')[mod(copyIndex('_2.copyVariable'), 2)]]"
//@        }
//@      }
//@    "_2.copyVariableCount": 10,
//@    "_2.notExported": [
//@      "bippity boppity boop",
//@      "abracadabra"
//@    ],
//@    "refersToCopyVariable": "[variables('_2.copyVariable')]",
} from 'modules/mod.json'

var aliasedFoo = foo
//@            "description": "Say hi to someone!",
//@    "aliasedFoo": "[variables('foo')]",
var aliasedBar = mod2.foo
//@    "aliasedBar": "[variables('_3.foo')]",

type fizzes = fizz[]
//@    "fizzes": {
//@      "type": "array",
//@      "items": {
//@        "$ref": "#/definitions/fizz"
//@      }
//@    },

param fizzParam mod2.fizz
//@    "fizzParam": {
//@      "$ref": "#/definitions/_3.fizz"
//@    }
output magicWord pop = refersToCopyVariable[3].value
//@    "magicWord": {
//@      "$ref": "#/definitions/pop",
//@      "value": "[variables('refersToCopyVariable')[3].value]"
//@    },

output greeting string = greet('friend')
//@    "greeting": {
//@      "type": "string",
//@      "value": "[__bicep.greet('friend')]"
//@    }

