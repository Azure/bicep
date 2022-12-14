// mandatory params
param dnsPrefix string
//@    "dnsPrefix": {
//@      "type": "string"
//@    },
param linuxAdminUsername string
//@    "linuxAdminUsername": {
//@      "type": "string"
//@    },
param sshRSAPublicKey string
//@    "sshRSAPublicKey": {
//@      "type": "string"
//@    },

@secure()
param servcePrincipalClientId string
//@    "servcePrincipalClientId": {
//@      "type": "securestring"
//@    },

@secure()
param servicePrincipalClientSecret string
//@    "servicePrincipalClientSecret": {
//@      "type": "securestring"
//@    },

// optional params
param clusterName string = 'aks101cluster'
//@    "clusterName": {
//@      "type": "string",
//@      "defaultValue": "aks101cluster"
//@    },
param location string = resourceGroup().location
//@    "location": {
//@      "type": "string",
//@      "defaultValue": "[resourceGroup().location]"
//@    },

@minValue(0)
//@      "minValue": 0
@maxValue(1023)
//@      "maxValue": 1023,
param osDiskSizeGB int = 0
//@    "osDiskSizeGB": {
//@      "type": "int",
//@      "defaultValue": 0,
//@    },

@minValue(1)
//@      "minValue": 1
@maxValue(50)
//@      "maxValue": 50,
param agentCount int = 3
//@    "agentCount": {
//@      "type": "int",
//@      "defaultValue": 3,
//@    },

param agentVMSize string = 'Standard_DS2_v2'
//@    "agentVMSize": {
//@      "type": "string",
//@      "defaultValue": "Standard_DS2_v2"
//@    }
// osType was a defaultValue with only one allowedValue, which seems strange?, could be a good TTK test

resource aks 'Microsoft.ContainerService/managedClusters@2020-03-01' = {
//@    {
//@      "type": "Microsoft.ContainerService/managedClusters",
//@      "apiVersion": "2020-03-01",
//@      "name": "[parameters('clusterName')]",
//@    }
    name: clusterName
    location: location
//@      "location": "[parameters('location')]",
    properties: {
//@      "properties": {
//@      }
        dnsPrefix: dnsPrefix
//@        "dnsPrefix": "[parameters('dnsPrefix')]",
        agentPoolProfiles: [
//@        "agentPoolProfiles": [
//@        ],
            {
//@          {
//@          }
                name: 'agentpool'
//@            "name": "agentpool",
                osDiskSizeGB: osDiskSizeGB
//@            "osDiskSizeGB": "[parameters('osDiskSizeGB')]",
                vmSize: agentVMSize
//@            "vmSize": "[parameters('agentVMSize')]",
                osType: 'Linux'
//@            "osType": "Linux",
                storageProfile: 'ManagedDisks'
//@            "storageProfile": "ManagedDisks"
            }
        ]
        linuxProfile: {
//@        "linuxProfile": {
//@        },
            adminUsername: linuxAdminUsername
//@          "adminUsername": "[parameters('linuxAdminUsername')]",
            ssh: {
//@          "ssh": {
//@          }
                publicKeys: [
//@            "publicKeys": [
//@            ]
                    {
//@              {
//@              }
                        keyData: sshRSAPublicKey
//@                "keyData": "[parameters('sshRSAPublicKey')]"
                    }
                ]
            }
        }
        servicePrincipalProfile: {
//@        "servicePrincipalProfile": {
//@        }
            clientId: servcePrincipalClientId
//@          "clientId": "[parameters('servcePrincipalClientId')]",
            secret: servicePrincipalClientSecret
//@          "secret": "[parameters('servicePrincipalClientSecret')]"
        }
    }
}

// fyi - dot property access (aks.fqdn) has not been spec'd
//output controlPlaneFQDN string = aks.properties.fqdn 
