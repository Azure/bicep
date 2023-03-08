
@sys.description('string output description')
//@        "description": "string output description"
output myStr string = 'hello'
//@    "myStr": {
//@      "type": "string",
//@      "metadata": {
//@      },
//@      "value": "hello"
//@    },

@sys.description('int output description')
//@        "description": "int output description"
output myInt int = 7
//@    "myInt": {
//@      "type": "int",
//@      "metadata": {
//@      },
//@      "value": 7
//@    },
output myOtherInt int = 20 / 13 + 80 % -4
//@    "myOtherInt": {
//@      "type": "int",
//@      "value": "[add(div(20, 13), mod(80, -4))]"
//@    },

@sys.description('bool output description')
//@        "description": "bool output description"
output myBool bool = !false
//@    "myBool": {
//@      "type": "bool",
//@      "metadata": {
//@      },
//@      "value": "[not(false())]"
//@    },
output myOtherBool bool = true
//@    "myOtherBool": {
//@      "type": "bool",
//@      "value": true
//@    },

@sys.description('object array description')
//@        "description": "object array description"
output suchEmpty array = [
//@    "suchEmpty": {
//@      "type": "array",
//@      "metadata": {
//@      },
//@      "value": []
//@    },
]

output suchEmpty2 object = {
//@    "suchEmpty2": {
//@      "type": "object",
//@      "value": {}
//@    },
}

@sys.description('object output description')
//@        "description": "object output description"
output obj object = {
//@    "obj": {
//@      "type": "object",
//@      "metadata": {
//@      },
//@      "value": {
//@      }
//@    },
  a: 'a'
//@        "a": "a",
  b: 12
//@        "b": 12,
  c: true
//@        "c": true,
  d: null
//@        "d": null,
  list: [
//@        "list": [
//@        ],
    1
//@          1,
    2
//@          2,
    3
//@          3,
    null
//@          null,
    {
//@          {}
    }
  ]
  obj: {
//@        "obj": {
//@        }
    nested: [
//@          "nested": [
//@          ]
      'hello'
//@            "hello"
    ]
  }
}

output myArr array = [
//@    "myArr": {
//@      "type": "array",
//@      "value": [
//@      ]
//@    },
  'pirates'
//@        "pirates",
  'say'
//@        "say",
   false ? 'arr2' : 'arr'
//@        "[if(false(), 'arr2', 'arr')]"
]

output rgLocation string = resourceGroup().location
//@    "rgLocation": {
//@      "type": "string",
//@      "value": "[resourceGroup().location]"
//@    },

output isWestUs bool = resourceGroup().location != 'westus' ? false : true
//@    "isWestUs": {
//@      "type": "bool",
//@      "value": "[if(not(equals(resourceGroup().location, 'westus')), false(), true())]"
//@    },

output expressionBasedIndexer string = {
//@    "expressionBasedIndexer": {
//@      "type": "string",
//@      "value": "[createObject('eastus', createObject('foo', true()), 'westus', createObject('foo', false()))[resourceGroup().location].foo]"
//@    },
  eastus: {
    foo: true
  }
  westus: {
    foo: false
  }
}[resourceGroup().location].foo

var secondaryKeyIntermediateVar = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01').secondaryKey
//@      "value": "[listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01').secondaryKey]"

output primaryKey string = listKeys(resourceId('Mock.RP/type', 'nigel'), '2020-01-01').primaryKey
//@    "primaryKey": {
//@      "type": "string",
//@      "value": "[listKeys(resourceId('Mock.RP/type', 'nigel'), '2020-01-01').primaryKey]"
//@    },
output secondaryKey string = secondaryKeyIntermediateVar
//@    "secondaryKey": {
//@      "type": "string",
//@    },

var varWithOverlappingOutput = 'hello'
//@    "varWithOverlappingOutput": "hello"
param paramWithOverlappingOutput string
//@    "paramWithOverlappingOutput": {
//@      "type": "string"
//@    }

output varWithOverlappingOutput string = varWithOverlappingOutput
//@    "varWithOverlappingOutput": {
//@      "type": "string",
//@      "value": "[variables('varWithOverlappingOutput')]"
//@    },
output paramWithOverlappingOutput string = paramWithOverlappingOutput
//@    "paramWithOverlappingOutput": {
//@      "type": "string",
//@      "value": "[parameters('paramWithOverlappingOutput')]"
//@    },

// top-level output loops are supported
output generatedArray array = [for i in range(0,10): i]
//@    "generatedArray": {
//@      "type": "array",
//@      "copy": {
//@        "count": "[length(range(0, 10))]",
//@        "input": "[range(0, 10)[copyIndex()]]"
//@      }
//@    }

