import {foo, fizz} from 'modules/mod.bicep'
//@    "1.buzz": {
//@      "type": "object",
//@      "properties": {
//@        "property": {
//@          "$ref": "#/definitions/1.pop",
//@          "nullable": true
//@        }
//@      },
//@      "metadata": {
//@        "__bicep_imported_from!": {
//@          "sourceTemplate": "modules/mod.bicep"
//@        }
//@      }
//@    },
//@    "1.pop": {
//@      "type": "string",
//@      "metadata": {
//@        "__bicep_imported_from!": {
//@          "sourceTemplate": "modules/mod.bicep"
//@        }
//@      }
//@    },
//@    "fizz": {
//@      "type": "array",
//@      "items": {
//@        "$ref": "#/definitions/1.buzz"
//@      },
//@      "metadata": {
//@        "__bicep_imported_from!": {
//@          "sourceTemplate": "modules/mod.bicep"
//@        }
//@      }
//@    }
//@    "1.bar": "[variables('1.baz')]",
//@    "1.baz": "quux",
//@    "foo": "[variables('1.bar')]",
import * as mod2 from 'modules/mod2.bicep'
//@    "3.fizz": {
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
//@    "3.foo": "bar",
import {
  'not-a-valid-bicep-identifier' as withInvalidIdentifier
//@      "minLength": 3,
//@    "withInvalidIdentifier": "value"
  refersToCopyVariable
//@      {
//@        "name": "2.copyVariable",
//@        "count": "[length(range(0, variables('2.copyVariableCount')))]",
//@        "input": {
//@          "count": "[copyIndex('2.copyVariable')]",
//@          "value": "[variables('2.notExported')[mod(copyIndex('2.copyVariable'), 2)]]"
//@        }
//@      }
//@    "2.copyVariableCount": 10,
//@    "2.notExported": [
//@      "bippity boppity boop",
//@      "abracadabra"
//@    ],
//@    "refersToCopyVariable": "[variables('2.copyVariable')]",
} from 'modules/mod.json'

var aliasedFoo = foo
//@    "aliasedFoo": "[variables('foo')]",

type fizzes = fizz[]
//@    "fizzes": {
//@      "type": "array",
//@      "items": {
//@        "$ref": "#/definitions/fizz"
//@      }
//@    },

