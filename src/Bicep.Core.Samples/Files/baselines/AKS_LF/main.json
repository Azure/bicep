{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "13329550984881458583"
    }
  },
  "parameters": {
    "dnsPrefix": {
      "type": "string"
    },
    "linuxAdminUsername": {
      "type": "string"
    },
    "sshRSAPublicKey": {
      "type": "string"
    },
    "servicePrincipalClientId": {
      "type": "securestring"
    },
    "servicePrincipalClientSecret": {
      "type": "securestring"
    },
    "clusterName": {
      "type": "string",
      "defaultValue": "aks101cluster"
    },
    "location": {
      "type": "string",
      "defaultValue": "[resourceGroup().location]"
    },
    "osDiskSizeGB": {
      "type": "int",
      "defaultValue": 0,
      "minValue": 0,
      "maxValue": 1023
    },
    "agentCount": {
      "type": "int",
      "defaultValue": 3,
      "minValue": 1,
      "maxValue": 50
    },
    "agentVMSize": {
      "type": "string",
      "defaultValue": "Standard_DS2_v2"
    }
  },
  "resources": [
    {
      "type": "Microsoft.ContainerService/managedClusters",
      "apiVersion": "2020-03-01",
      "name": "[parameters('clusterName')]",
      "location": "[parameters('location')]",
      "properties": {
        "dnsPrefix": "[parameters('dnsPrefix')]",
        "agentPoolProfiles": [
          {
            "name": "agentpool",
            "osDiskSizeGB": "[parameters('osDiskSizeGB')]",
            "vmSize": "[parameters('agentVMSize')]",
            "osType": "Linux",
            "storageProfile": "ManagedDisks"
          }
        ],
        "linuxProfile": {
          "adminUsername": "[parameters('linuxAdminUsername')]",
          "ssh": {
            "publicKeys": [
              {
                "keyData": "[parameters('sshRSAPublicKey')]"
              }
            ]
          }
        },
        "servicePrincipalProfile": {
          "clientId": "[parameters('servicePrincipalClientId')]",
          "secret": "[parameters('servicePrincipalClientSecret')]"
        }
      }
    }
  ]
}