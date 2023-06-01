metadata name = 'Sample module'
metadata description = 'Sample summary'
metadata owner = 'test'

// TODO: Remove "sys." everywhere once bicep v0.18 ships
@sys.description('The dns prefix')
param dnsPrefix string

@sys.description('The linux administrator username')
param linuxAdminUsername string

@sys.description('The RSA public key for SSH')
param sshRSAPublicKey string

@sys.description('The service principal client ID')
param servicePrincipalClientId string

@sys.description('The service principal client secret')
@secure()
param servicePrincipalClientSecret string

// optional params
@sys.description('The cluster name')
param clusterName string = 'aks101cluster'

@sys.description('The deployment location')
param location string = resourceGroup().location

@sys.description('''
The OS disk size (in GB)
- Minimum value is 0
- Maximum value is 1023
''')
@minValue(0)
@maxValue(1023)
param osDiskSizeGB int

@sys.description('The agent count')
@minValue(1)
@maxValue(50)
// TODO: Causes error during build
param agentCount int = 0

@sys.description('The agent VM size')
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

@sys.description('The control plane FQDN')
output controlPlaneFQDN string = aks.properties.fqdn
