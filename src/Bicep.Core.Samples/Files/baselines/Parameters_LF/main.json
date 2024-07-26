{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "4645020002980758832"
    }
  },
  "parameters": {
    "myString": {
      "type": "string",
      "metadata": {
        "description": "this is my multi line\ndescription for my myString\n"
      }
    },
    "myInt": {
      "type": "int"
    },
    "myBool": {
      "type": "bool"
    },
    "myString2": {
      "type": "string",
      "defaultValue": "string value",
      "metadata": {
        "description": "this is myString2"
      }
    },
    "myInt2": {
      "type": "int",
      "defaultValue": 42
    },
    "myTruth": {
      "type": "bool",
      "defaultValue": true
    },
    "myFalsehood": {
      "type": "bool",
      "defaultValue": false
    },
    "myEscapedString": {
      "type": "string",
      "defaultValue": "First line\r\nSecond\ttabbed\tline"
    },
    "foo": {
      "type": "object",
      "defaultValue": {
        "enabled": true,
        "name": "this is my object",
        "priority": 3,
        "info": {
          "a": "b"
        },
        "empty": {},
        "array": [
          "string item",
          12,
          true,
          [
            "inner",
            false
          ],
          {
            "a": "b"
          }
        ]
      },
      "metadata": {
        "description": "this is foo",
        "another": "just for fun"
      }
    },
    "myArrayParam": {
      "type": "array",
      "defaultValue": [
        "a",
        "b",
        "c"
      ]
    },
    "password": {
      "type": "securestring"
    },
    "secretObject": {
      "type": "secureObject"
    },
    "storageSku": {
      "type": "string",
      "allowedValues": [
        "Standard_LRS",
        "Standard_GRS"
      ]
    },
    "intEnum": {
      "type": "int",
      "allowedValues": [
        1,
        2,
        3
      ]
    },
    "storageName": {
      "type": "string",
      "minLength": 3,
      "maxLength": 24
    },
    "someArray": {
      "type": "array",
      "minLength": 3,
      "maxLength": 24
    },
    "emptyMetadata": {
      "type": "string",
      "metadata": {}
    },
    "description": {
      "type": "string",
      "metadata": {
        "description": "my description"
      }
    },
    "description2": {
      "type": "string",
      "metadata": {
        "description": "my description"
      }
    },
    "additionalMetadata": {
      "type": "string",
      "metadata": {
        "description": "my description",
        "a": 1,
        "b": true,
        "c": [],
        "d": {
          "test": "abc"
        }
      }
    },
    "someParameter": {
      "type": "securestring",
      "allowedValues": [
        "one",
        "two",
        "three"
      ],
      "metadata": {
        "description": "Name of the storage account"
      },
      "minLength": 3,
      "maxLength": 24
    },
    "defaultExpression": {
      "type": "bool",
      "defaultValue": "[not(equals(18, or(true(), false())))]"
    },
    "stringLiteral": {
      "type": "string",
      "allowedValues": [
        "abc",
        "def"
      ]
    },
    "stringLiteralWithAllowedValuesSuperset": {
      "type": "string",
      "defaultValue": "[parameters('stringLiteral')]",
      "allowedValues": [
        "abc",
        "def",
        "ghi"
      ]
    },
    "decoratedString": {
      "type": "securestring",
      "allowedValues": [
        "Apple",
        "Banana"
      ],
      "minLength": 2,
      "maxLength": 10
    },
    "decoratedInt": {
      "type": "int",
      "defaultValue": 123,
      "minValue": 100
    },
    "negativeValues": {
      "type": "int",
      "minValue": -10,
      "maxValue": -3
    },
    "decoratedBool": {
      "type": "bool",
      "defaultValue": "[not(equals(and(true(), false()), true()))]",
      "metadata": {
        "description": "A boolean.",
        "foo": "something",
        "bar": [
          {},
          true,
          123
        ]
      }
    },
    "decoratedObject": {
      "type": "secureObject",
      "defaultValue": {
        "enabled": true,
        "name": "this is my object",
        "priority": 3,
        "info": {
          "a": "b"
        },
        "empty": {},
        "array": [
          "string item",
          12,
          true,
          [
            "inner",
            false
          ],
          {
            "a": "b"
          }
        ]
      }
    },
    "decoratedArray": {
      "type": "array",
      "defaultValue": [
        "[utcNow()]",
        "[newGuid()]"
      ],
      "metadata": {
        "description": "An array."
      },
      "maxLength": 20
    },
    "nameofParam": {
      "type": "string",
      "defaultValue": "decoratedArray"
    }
  },
  "resources": []
}