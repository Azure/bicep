{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "languageVersion": "2.0",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "3381412150001193210"
    },
    "__bicep_exported_variables!": [
      {
        "name": "foo"
      }
    ]
  },
  "definitions": {
    "fizz": {
      "type": "string",
      "allowedValues": [
        "buzz"
      ],
      "metadata": {
        "__bicep_export!": true
      }
    }
  },
  "variables": {
    "foo": "bar"
  },
  "resources": {}
}