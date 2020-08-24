// mandatory params
param dnsPrefix string
param linuxAdminUsername string
param sshRSAPublicKey string
param servcePrincipalClientId string {
    secure: true
}
param servicePrincipalClientSecret string {
    secure: true
}

// optional params
param clusterName string = 'aks101cluster'
param location string = resourceGroup().location
param osDiskSizeGB int {
    default: 0
    minValue: 0
    maxValue: 1023
}
param agentCount int {
    default: 3
    minValue: 1
    maxValue: 50
}
param agentVMSize string = 'Standard_DS2_v2'
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
