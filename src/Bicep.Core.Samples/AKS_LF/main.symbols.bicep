// mandatory params
param dnsPrefix string
//@[6:15) Parameter dnsPrefix. Type: string. Declaration start char: 0, length: 23
param linuxAdminUsername string
//@[6:24) Parameter linuxAdminUsername. Type: string. Declaration start char: 0, length: 32
param sshRSAPublicKey string
//@[6:21) Parameter sshRSAPublicKey. Type: string. Declaration start char: 0, length: 29
param servcePrincipalClientId string {
//@[6:29) Parameter servcePrincipalClientId. Type: string. Declaration start char: 0, length: 58
    secure: true
}
param servicePrincipalClientSecret string {
//@[6:34) Parameter servicePrincipalClientSecret. Type: string. Declaration start char: 0, length: 64
    secure: true
}

// optional params
param clusterName string = 'aks101cluster'
//@[6:17) Parameter clusterName. Type: string. Declaration start char: 0, length: 43
param location string = resourceGroup().location
//@[6:14) Parameter location. Type: string. Declaration start char: 0, length: 49
param osDiskSizeGB int {
//@[6:18) Parameter osDiskSizeGB. Type: int. Declaration start char: 0, length: 77
    default: 0
    minValue: 0
    maxValue: 1023
}
param agentCount int {
//@[6:16) Parameter agentCount. Type: int. Declaration start char: 0, length: 73
    default: 3
    minValue: 1
    maxValue: 50
}
param agentVMSize string = 'Standard_DS2_v2'
//@[6:17) Parameter agentVMSize. Type: string. Declaration start char: 0, length: 45
// osType was a defaultValue with only one allowedValue, which seems strange?, could be a good TTK test

resource aks 'Microsoft.ContainerService/managedClusters@2020-03-01' = {
//@[9:12) Resource aks. Type: Microsoft.ContainerService/managedClusters@2020-03-01. Declaration start char: 0, length: 827
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
