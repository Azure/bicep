// mandatory params
param dnsPrefix string
//@[6:15] Parameter dnsPrefix
param linuxAdminUsername string
//@[6:24] Parameter linuxAdminUsername
param sshRSAPublicKey string
//@[6:21] Parameter sshRSAPublicKey
param servcePrincipalClientId string {
//@[6:29] Parameter servcePrincipalClientId
    secure: true
}
param servicePrincipalClientSecret string {
//@[6:34] Parameter servicePrincipalClientSecret
    secure: true
}

// optional params
param clusterName string = 'aks101cluster'
//@[6:17] Parameter clusterName
param location string = resourceGroup().location
//@[6:14] Parameter location
param osDiskSizeGB int {
//@[6:18] Parameter osDiskSizeGB
    default: 0
    minValue: 0
    maxValue: 1023
}
param agentCount int {
//@[6:16] Parameter agentCount
    default: 3
    minValue: 1
    maxValue: 50
}
param agentVMSize string = 'Standard_DS2_v2'
//@[6:17] Parameter agentVMSize
// osType was a defaultValue with only one allowedValue, which seems strange?, could be a good TTK test

resource aks 'Microsoft.ContainerService/managedClusters@2020-03-01' = {
//@[9:12] Resource aks
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
