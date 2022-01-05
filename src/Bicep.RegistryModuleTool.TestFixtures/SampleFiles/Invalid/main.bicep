param dnsPrefix string

@description('The linux administrator username')
param linuxAdminUsername string

@description('The RSA public key for SSH')
param sshRSAPublicKey string

@description('The service principal client ID')
param servicePrincipalClientId string

@secure()
param servicePrincipalClientSecret string

// optional params
@description('The cluster name')
param clusterName string = 'aks101cluster'

@description('The deployment location')
param location string = resourceGroup().location

@description('The OS disk size (in GB)')
@minValue(0)
@maxValue(1023)
param osDiskSizeGB int

@description('The agent count')
@minValue(1)
@maxValue(50)
param agentCount int = 0

@description('The agent VM size')
param agentVMSize string = 'Standard_DS2_v2'
// osType was a defaultValue with only one allowedValue, which seems strange?, could be a good TTK test

resource aks 'Microsoft.ContainerService/managedClusters@2020-09-01' = {
  name: clusterName
  location: location
  properties: {
    dnsPrefix: dnsPrefix
    agentPoolProfiles: [
      {
        name: 'agentpool'
        osDiskSizeGB: osDiskSizeGB
        count: agentCount
        vmSize: agentVMSize
        osType: 'Linux'
        mode: 'System'
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
      clientId: servicePrincipalClientId
      secret: servicePrincipalClientSecret
    }
  }
}

output controlPlaneFQDN string = aks.properties.fqdn
