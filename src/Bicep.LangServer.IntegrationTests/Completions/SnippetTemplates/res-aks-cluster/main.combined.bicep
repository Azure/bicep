resource aksCluster 'Microsoft.ContainerService/managedClusters@2021-03-01' = {
  name: 'aksCluster'
  location: resourceGroup().location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    kubernetesVersion: ''1.19.7''
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
      adminUsername: 'adminUsername'
      ssh: {
        publicKeys: [
          {
            keyData: 'keyData'
          }
        ]
      }
    }
  }
}
