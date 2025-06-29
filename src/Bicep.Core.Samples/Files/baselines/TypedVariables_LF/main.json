{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "languageVersion": "2.0",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "6383359498761660162"
    },
    "__bicep_exported_variables!": [
      {
        "name": "exporteInlineType",
        "type": {
          "type": "object",
          "properties": {
            "foo": {
              "type": "string"
            },
            "bar": {
              "type": "int"
            }
          }
        }
      },
      {
        "name": "exportedString",
        "type": {
          "type": "string"
        }
      },
      {
        "name": "exportedTypeRef",
        "type": {
          "$ref": "#/definitions/FooType"
        }
      }
    ]
  },
  "definitions": {
    "FooType": {
      "type": "object",
      "properties": {
        "foo": {
          "type": "string"
        },
        "bar": {
          "type": "int"
        }
      }
    }
  },
  "variables": {
    "exportedString": "foo",
    "exporteInlineType": {
      "foo": "abc",
      "bar": 123
    },
    "exportedTypeRef": {
      "foo": "abc",
      "bar": 123
    },
    "unExported": {
      "foo": "abc",
      "bar": 123
    }
  },
  "resources": {}
}