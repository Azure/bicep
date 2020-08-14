﻿// mandatory params
parameter dnsPrefix string
parameter linuxAdminUsername string
parameter sshRSAPublicKey string
parameter servcePrincipalClientId string {
    secure: true
}
parameter servicePrincipalClientSecret string {
    secure: true
}

// optional params
parameter clusterName string = 'aks101cluster'
parameter location string = resourceGroup().location
parameter osDiskSizeGB int {
    defaultValue: 0
    minValue: 0
    maxValue: 1023
}
parameter agentCount int {
    defaultValue: 3
    minValue: 1
    maxValue: 50
}
parameter agentVMSize string = 'Standard_DS2_v2'
// osType was a defaultValue with only one allowedValue, which seems strange?, could be a good TTK test

resource aks 'Microsoft.ContainerService/managedClusters@2020-03-01' = {
    name: clusterName
    location: location
    properties: {
        dnsPrefix: dnsPrefix
        agentPoolProfiles: [
            {
                name: 'agentpool'
                osDiskSizeGB: osDiskSizeGB
                vmSize: agentVMSize
                osType: 'Linux'
                storageProfile: 'ManagedDisks'
            }
        ]
        linuxProfile: {
            adminUsername: linuxAdminUsername
            ssh: {
                publicKeys: [
                    {
                        keyData: sshRSAPublicKey
                    }
                ]
            }
        }
        servicePrincipalProfile: {
            clientId: servcePrincipalClientId
            secret: servicePrincipalClientSecret
        }
    }
}

// fyi - dot property access (aks.fqdn) has not been spec'd
//output controlPlaneFQDN string = aks.properties.fqdn 