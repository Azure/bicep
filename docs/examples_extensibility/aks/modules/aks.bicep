param location string = resourceGroup().location
param baseName string
param dnsPrefix string
param linuxAdminUsername string
param sshRSAPublicKey string

var osDiskSizeGB = 0
var agentCount = 3
var agentVMSize = 'standard_f2s_v2' //'Standard_DS2_v2'

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
        #disable-next-line BCP036
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
