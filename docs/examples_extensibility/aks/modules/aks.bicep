param location string = resourceGroup().location
param baseName string
param dnsPrefix string
param linuxAdminUsername string
param sshRSAPublicKey string

var osDiskSizeGB = 0
var agentCount = 3
var agentVMSize = 'Standard_DS2_v2'

resource aks 'Microsoft.ContainerService/managedClusters@2020-09-01' = {
  name: baseName
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
  }
  identity: {
    type: 'SystemAssigned'
  }
}

output fqdn string = aks.properties.fqdn
output kubeConfig string = aks.listClusterAdminCredential().kubeconfigs[0].value
