// mandatory params
param dnsPrefix string
//@[line01->line11]     "dnsPrefix": {
//@[line01->line12]       "type": "string"
//@[line01->line13]     },
param linuxAdminUsername string
//@[line02->line14]     "linuxAdminUsername": {
//@[line02->line15]       "type": "string"
//@[line02->line16]     },
param sshRSAPublicKey string
//@[line03->line17]     "sshRSAPublicKey": {
//@[line03->line18]       "type": "string"
//@[line03->line19]     },

@secure()
param servcePrincipalClientId string
//@[line06->line20]     "servcePrincipalClientId": {
//@[line06->line21]       "type": "securestring"
//@[line06->line22]     },

@secure()
param servicePrincipalClientSecret string
//@[line09->line23]     "servicePrincipalClientSecret": {
//@[line09->line24]       "type": "securestring"
//@[line09->line25]     },

// optional params
param clusterName string = 'aks101cluster'
//@[line12->line26]     "clusterName": {
//@[line12->line27]       "type": "string",
//@[line12->line28]       "defaultValue": "aks101cluster"
//@[line12->line29]     },
param location string = resourceGroup().location
//@[line13->line30]     "location": {
//@[line13->line31]       "type": "string",
//@[line13->line32]       "defaultValue": "[resourceGroup().location]"
//@[line13->line33]     },

@minValue(0)
//@[line15->line38]       "minValue": 0
@maxValue(1023)
//@[line16->line37]       "maxValue": 1023,
param osDiskSizeGB int = 0
//@[line17->line34]     "osDiskSizeGB": {
//@[line17->line35]       "type": "int",
//@[line17->line36]       "defaultValue": 0,
//@[line17->line39]     },

@minValue(1)
//@[line19->line44]       "minValue": 1
@maxValue(50)
//@[line20->line43]       "maxValue": 50,
param agentCount int = 3
//@[line21->line40]     "agentCount": {
//@[line21->line41]       "type": "int",
//@[line21->line42]       "defaultValue": 3,
//@[line21->line45]     },

param agentVMSize string = 'Standard_DS2_v2'
//@[line23->line46]     "agentVMSize": {
//@[line23->line47]       "type": "string",
//@[line23->line48]       "defaultValue": "Standard_DS2_v2"
//@[line23->line49]     }
// osType was a defaultValue with only one allowedValue, which seems strange?, could be a good TTK test

resource aks 'Microsoft.ContainerService/managedClusters@2020-03-01' = {
//@[line26->line52]     {
//@[line26->line53]       "type": "Microsoft.ContainerService/managedClusters",
//@[line26->line54]       "apiVersion": "2020-03-01",
//@[line26->line55]       "name": "[parameters('clusterName')]",
//@[line26->line83]     }
    name: clusterName
    location: location
//@[line28->line56]       "location": "[parameters('location')]",
    properties: {
//@[line29->line57]       "properties": {
//@[line29->line82]       }
        dnsPrefix: dnsPrefix
//@[line30->line58]         "dnsPrefix": "[parameters('dnsPrefix')]",
        agentPoolProfiles: [
//@[line31->line59]         "agentPoolProfiles": [
//@[line31->line67]         ],
            {
//@[line32->line60]           {
//@[line32->line66]           }
                name: 'agentpool'
//@[line33->line61]             "name": "agentpool",
                osDiskSizeGB: osDiskSizeGB
//@[line34->line62]             "osDiskSizeGB": "[parameters('osDiskSizeGB')]",
                vmSize: agentVMSize
//@[line35->line63]             "vmSize": "[parameters('agentVMSize')]",
                osType: 'Linux'
//@[line36->line64]             "osType": "Linux",
                storageProfile: 'ManagedDisks'
//@[line37->line65]             "storageProfile": "ManagedDisks"
            }
        ]
        linuxProfile: {
//@[line40->line68]         "linuxProfile": {
//@[line40->line77]         },
            adminUsername: linuxAdminUsername
//@[line41->line69]           "adminUsername": "[parameters('linuxAdminUsername')]",
            ssh: {
//@[line42->line70]           "ssh": {
//@[line42->line76]           }
                publicKeys: [
//@[line43->line71]             "publicKeys": [
//@[line43->line75]             ]
                    {
//@[line44->line72]               {
//@[line44->line74]               }
                        keyData: sshRSAPublicKey
//@[line45->line73]                 "keyData": "[parameters('sshRSAPublicKey')]"
                    }
                ]
            }
        }
        servicePrincipalProfile: {
//@[line50->line78]         "servicePrincipalProfile": {
//@[line50->line81]         }
            clientId: servcePrincipalClientId
//@[line51->line79]           "clientId": "[parameters('servcePrincipalClientId')]",
            secret: servicePrincipalClientSecret
//@[line52->line80]           "secret": "[parameters('servicePrincipalClientSecret')]"
        }
    }
}

// fyi - dot property access (aks.fqdn) has not been spec'd
//output controlPlaneFQDN string = aks.properties.fqdn 
