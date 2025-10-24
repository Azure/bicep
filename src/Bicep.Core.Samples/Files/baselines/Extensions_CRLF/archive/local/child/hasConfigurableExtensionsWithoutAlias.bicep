{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "languageVersion": "2.2-experimental",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_EXPERIMENTAL_WARNING": "This template uses ARM features that are experimental. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.",
    "_EXPERIMENTAL_FEATURES_ENABLED": [
      "Enable defining extension configs for modules"
    ],
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "12485839995628084055"
    }
  },
  "extensions": {
    "kubernetes": {
      "name": "Kubernetes",
      "version": "1.0.0",
      "config": {
        "namespace": {
          "defaultValue": "nsInsideModule"
        }
      }
    }
  },
  "resources": {}
}