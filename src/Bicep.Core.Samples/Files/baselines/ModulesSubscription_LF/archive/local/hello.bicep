{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "9518912405470532169"
    }
  },
  "parameters": {
    "scriptName": {
      "type": "string"
    }
  },
  "resources": [
    {
      "type": "Microsoft.Resources/deploymentScripts",
      "apiVersion": "2020-10-01",
      "name": "[parameters('scriptName')]",
      "kind": "AzurePowerShell",
      "location": "[resourceGroup().location]",
      "properties": {
        "azPowerShellVersion": "3.0",
        "retentionInterval": "PT6H",
        "scriptContent": "      Write-Output 'Hello World!'\n"
      }
    }
  ],
  "outputs": {
    "myOutput": {
      "type": "string",
      "value": "[parameters('scriptName')]"
    }
  }
}