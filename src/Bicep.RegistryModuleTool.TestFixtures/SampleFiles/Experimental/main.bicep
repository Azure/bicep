@description('The dns prefix')
param dnsPrefix string

@description('The linux administrator username')
param linuxAdminUsername string

@description('The RSA public key for SSH')
param sshRSAPublicKey string

@description('The service principal client ID')
param servicePrincipalClientId string

@description('The service principal client secret')
@secure()
param servicePrincipalClientSecret string

// optional params
@description('The cluster name')
param clusterName string = 'aks101cluster'

@description('The deployment location')
param location string = resourceGroup().location

@description('''
The OS disk size (in GB)
- Minimum value is 0
- Maximum value is 1023
''')
@minValue(0)
@maxValue(1023)
type MinDiskSizeGB = int

type DiskSizeGB = MinDiskSizeGB

param osDiskSizeGB DiskSizeGB

@description('The agent count')
@minValue(1)
@maxValue(50)
param agentCount int = 1

@description('The agent VM size')
@allowed(['Standard_DS2_v2', 'Standard_DS4_v2'])
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

@description('The control plane FQDN')
output controlPlaneFQDN string = aks.properties.fqdn

output osDiskSizeGB DiskSizeGB = osDiskSizeGB
