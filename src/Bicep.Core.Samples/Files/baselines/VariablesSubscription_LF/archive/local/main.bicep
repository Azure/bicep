{
  "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "18132682572862864781"
    }
  },
  "variables": {
    "deploymentLocation": "[deployment().location]",
    "scopesWithArmRepresentation": {
      "tenant": "[tenant()]",
      "subscription": "[subscription()]"
    }
  },
  "resources": []
}