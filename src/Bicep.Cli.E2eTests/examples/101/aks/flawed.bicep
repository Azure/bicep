// mandatory params
param dnsPrefix string
param linuxAdminUsername string
param sshRSAPublicKey string
param servicePrincipalClientId string
param servicePrincipalClientSecret string {
  secure: true
}

// optional params
param clusterName string = 'aks101cluster'
param location string = resourceGroup().location
param osDiskSizeGB int {
  default: 0 // a value of zero means they will use the default value (which is 128 as of this writing)
  minValue: 0
  maxValue: 1023
}

param agentCount int {
  default: 3
  minValue: 1
  maxValue: 50
}
param agentVMSize string = 'Standard_DS2_v2
// osType was a defaultValue with only one allowedValue, which seems strange?, could be a good TTK test

resource aks 'Microsoft.ContainerService/managedClusters@2020-09-01' = {
  name: clusterName
  location: location
  properties: {
    dnsPrefix: dnsPrefix
    agentPoolProfiles: [
      {
        name: 'agentpool'
        osDiskSizeGB: osDiskSizeGb
        count: agentCount
        vmSize: agentVMSize
        osType: 'Linux'
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

