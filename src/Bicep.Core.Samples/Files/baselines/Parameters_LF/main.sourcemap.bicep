/*
  This is a block comment.
*/

// parameters without default value
@sys.description('''
//@      "metadata": {
//@        "description": "this is my multi line\ndescription for my myString\n"
//@      }
this is my multi line
description for my myString
''')
param myString string
//@    "myString": {
//@      "type": "string",
//@    },
param myInt int
//@    "myInt": {
//@      "type": "int"
//@    },
param myBool bool
//@    "myBool": {
//@      "type": "bool"
//@    },

// parameters with default value
@sys.description('this is myString2')
//@        "description": "this is myString2"
@metadata({
//@      "metadata": {
//@      }
  description: 'overwrite but still valid'
})
param myString2 string = 'string value'
//@    "myString2": {
//@      "type": "string",
//@      "defaultValue": "string value",
//@    },
param myInt2 int = 42
//@    "myInt2": {
//@      "type": "int",
//@      "defaultValue": 42
//@    },
param myTruth bool = true
//@    "myTruth": {
//@      "type": "bool",
//@      "defaultValue": true
//@    },
param myFalsehood bool = false
//@    "myFalsehood": {
//@      "type": "bool",
//@      "defaultValue": false
//@    },
param myEscapedString string = 'First line\r\nSecond\ttabbed\tline'
//@    "myEscapedString": {
//@      "type": "string",
//@      "defaultValue": "First line\r\nSecond\ttabbed\tline"
//@    },

// object default value
@sys.description('this is foo')
//@        "description": "this is foo",
@metadata({
//@      "metadata": {
//@      }
  description: 'overwrite but still valid'
  another: 'just for fun'
//@        "another": "just for fun"
})
param foo object = {
//@    "foo": {
//@      "type": "object",
//@      "defaultValue": {
//@      },
//@    },
  enabled: true
//@        "enabled": true,
  name: 'this is my object'
//@        "name": "this is my object",
  priority: 3
//@        "priority": 3,
  info: {
//@        "info": {
//@        },
    a: 'b'
//@          "a": "b"
  }
  empty: {
//@        "empty": {},
  }
  array: [
//@        "array": [
//@        ]
    'string item'
//@          "string item",
    12
//@          12,
    true
//@          true,
    [
//@          [
//@          ],
      'inner'
//@            "inner",
      false
//@            false
    ]
    {
//@          {
//@          }
      a: 'b'
//@            "a": "b"
    }
  ]
}

// array default value
param myArrayParam array = [
//@    "myArrayParam": {
//@      "type": "array",
//@      "defaultValue": [
//@      ]
//@    },
  'a'
//@        "a",
  'b'
//@        "b",
  'c'
//@        "c"
]

// secure string
@secure()
//@      "type": "securestring"
param password string
//@    "password": {
//@    },

// secure object
@secure()
//@      "type": "secureObject"
param secretObject object
//@    "secretObject": {
//@    },

// enum parameter
@allowed([
//@      "allowedValues": [
//@      ]
  'Standard_LRS'
//@        "Standard_LRS",
  'Standard_GRS'
//@        "Standard_GRS"
])
param storageSku string
//@    "storageSku": {
//@      "type": "string",
//@    },

@allowed([
//@      "allowedValues": [
//@      ]
  1
//@        1,
  2
//@        2,
  3
//@        3
])
param intEnum int
//@    "intEnum": {
//@      "type": "int",
//@    },

// length constraint on a string
@minLength(3)
//@      "minLength": 3,
@maxLength(24)
//@      "maxLength": 24
param storageName string
//@    "storageName": {
//@      "type": "string",
//@    },

// length constraint on an array
@minLength(3)
//@      "minLength": 3,
@maxLength(24)
//@      "maxLength": 24
param someArray array
//@    "someArray": {
//@      "type": "array",
//@    },

// empty metadata
@metadata({})
//@      "metadata": {}
param emptyMetadata string
//@    "emptyMetadata": {
//@      "type": "string",
//@    },

// description
@metadata({
//@      "metadata": {
//@      }
  description: 'my description'
//@        "description": "my description"
})
param description string
//@    "description": {
//@      "type": "string",
//@    },

@sys.description('my description')
//@      "metadata": {
//@        "description": "my description"
//@      }
param description2 string
//@    "description2": {
//@      "type": "string",
//@    },

// random extra metadata
@metadata({
//@      "metadata": {
//@      }
  description: 'my description'
//@        "description": "my description",
  a: 1
//@        "a": 1,
  b: true
//@        "b": true,
  c: [
//@        "c": [],
  ]
  d: {
//@        "d": {
//@        }
    test: 'abc'
//@          "test": "abc"
  }
})
param additionalMetadata string
//@    "additionalMetadata": {
//@      "type": "string",
//@    },

// all modifiers together
@secure()
//@      "type": "securestring",
@minLength(3)
//@      "minLength": 3,
@maxLength(24)
//@      "maxLength": 24
@allowed([
//@      "allowedValues": [
//@      ],
  'one'
//@        "one",
  'two'
//@        "two",
  'three'
//@        "three"
])
@metadata({
//@      "metadata": {
//@      },
  description: 'Name of the storage account'
//@        "description": "Name of the storage account"
})
param someParameter string
//@    "someParameter": {
//@    },

param defaultExpression bool = 18 != (true || false)
//@    "defaultExpression": {
//@      "type": "bool",
//@      "defaultValue": "[not(equals(18, or(true(), false())))]"
//@    },

@allowed([
//@      "allowedValues": [
//@      ]
  'abc'
//@        "abc",
  'def'
//@        "def"
])
param stringLiteral string
//@    "stringLiteral": {
//@      "type": "string",
//@    },

@allowed(
    // some comment
    [
//@      "allowedValues": [
//@      ]
  'abc'
//@        "abc",
  'def'
//@        "def",
  'ghi'
//@        "ghi"
])
param stringLiteralWithAllowedValuesSuperset string = stringLiteral
//@    "stringLiteralWithAllowedValuesSuperset": {
//@      "type": "string",
//@      "defaultValue": "[parameters('stringLiteral')]",
//@    },

@secure()
//@      "type": "securestring",
@minLength(2)
//@      "minLength": 2,
  @maxLength(10)
//@      "maxLength": 10
@allowed([
//@      "allowedValues": [
//@      ],
  'Apple'
//@        "Apple",
  'Banana'
//@        "Banana"
])
param decoratedString string
//@    "decoratedString": {
//@    },

@minValue(100)
//@      "minValue": 100
param decoratedInt int = 123
//@    "decoratedInt": {
//@      "type": "int",
//@      "defaultValue": 123,
//@    },

// negative integer literals are allowed as decorator values
@minValue(-10)
//@      "minValue": -10,
@maxValue(-3)
//@      "maxValue": -3
param negativeValues int
//@    "negativeValues": {
//@      "type": "int",
//@    },

@sys.description('A boolean.')
//@        "description": "A boolean.",
@metadata({
//@      "metadata": {
//@      }
    description: 'I will be overrode.'
    foo: 'something'
//@        "foo": "something",
    bar: [
//@        "bar": [
//@        ]
        {          }
//@          {},
        true
//@          true,
        123
//@          123
    ]
})
param decoratedBool bool = /* comment1 */ /* comment2*/      /* comment3 */ /* comment4 */ (true && false) != true
//@    "decoratedBool": {
//@      "type": "bool",
//@      "defaultValue": "[not(equals(and(true(), false()), true()))]",
//@    },

@secure()
//@      "type": "secureObject",
param decoratedObject object = {
//@    "decoratedObject": {
//@      "defaultValue": {
//@      }
//@    },
  enabled: true
//@        "enabled": true,
  name: 'this is my object'
//@        "name": "this is my object",
  priority: 3
//@        "priority": 3,
  info: {
//@        "info": {
//@        },
    a: 'b'
//@          "a": "b"
  }
  empty: {
//@        "empty": {},
  }
  array: [
//@        "array": [
//@        ]
    'string item'
//@          "string item",
    12
//@          12,
    true
//@          true,
    [
//@          [
//@          ],
      'inner'
//@            "inner",
      false
//@            false
    ]
    {
//@          {
//@          }
      a: 'b'
//@            "a": "b"
    }
  ]
}

@sys.metadata({
//@      "metadata": {
//@      },
    description: 'I will be overrode.'
})
@sys.maxLength(20)
//@      "maxLength": 20
@sys.description('An array.')
//@        "description": "An array."
param decoratedArray array = [
//@    "decoratedArray": {
//@      "type": "array",
//@      "defaultValue": [
//@      ],
//@    },
    utcNow()
//@        "[utcNow()]",
    newGuid()
//@        "[newGuid()]"
]

param nameofParam string = nameof(decoratedArray)
//@    "nameofParam": {
//@      "type": "string",
//@      "defaultValue": "decoratedArray"
//@    }

