{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "languageVersion": "2.1-experimental",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_EXPERIMENTAL_WARNING": "This template uses ARM features that are experimental. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.",
    "_EXPERIMENTAL_FEATURES_ENABLED": [
      "Asserts"
    ],
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "13061541748522586682"
    }
  },
  "parameters": {
    "location": {
      "type": "string"
    }
  },
  "variables": {
    "obj": {
      "prop": "juan",
      "nested": {
        "nestedProp": "nestedPropValue",
        "nestedArray": [
          1,
          2,
          3
        ],
        "loc": "[parameters('location')]"
      }
    }
  },
  "resources": {},
  "asserts": {
    "accessObj": "[equals(variables('obj').prop, 'juan')]",
    "accessNestedProp": "[equals(variables('obj').nested.nestedProp, 'nestedPropValue')]",
    "accessNestedPropArray": "[equals(variables('obj').nested.nestedArray, createArray(1, 2, 4))]"
  }
}