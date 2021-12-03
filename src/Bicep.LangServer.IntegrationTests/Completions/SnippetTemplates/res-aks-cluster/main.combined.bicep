// $1 = aksCluster
// $2 = 'name'
// $3 = 1.19.7
// $4 = 'dnsPrefix'
// $5 = 3
// $6 = 'Standard_DS2_v2'
// $7 = adminUsername
// $8 = 'REQUIRED'

param adminUsername string
param location string

resource aksCluster 'Microsoft.ContainerService/managedClusters@2021-03-01' = {
  name: 'name'
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    kubernetesVersion: '1.19.7'
    dnsPrefix: 'dnsPrefix'
    enableRBAC: true
    agentPoolProfiles: [
      {
        name: 'agentpool'
        count: 3
        vmSize: 'Standard_DS2_v2'
        osType: 'Linux'
        mode: 'System'
      }
    ]
    linuxProfile: {
      adminUsername: adminUsername
      ssh: {
        publicKeys: [
          {
            keyData: 'REQUIRED'
          }
        ]
      }
    }
  }
}
// Insert snippet here

