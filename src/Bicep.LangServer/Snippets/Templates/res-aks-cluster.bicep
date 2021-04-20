// Kubernetes Service Cluster
resource ${1:aksCluster} 'Microsoft.ContainerService/managedClusters@2021-03-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    kubernetesVersion: '${3|1.19.7,1.19.6,1.18.14,1.18.10,1.17.16,1.17.13|}'
    dnsPrefix: ${4:'dnsprefix'}
    enableRBAC: true
    agentPoolProfiles: [
      {
        name: 'agentpool'
        count: ${5:3}
        vmSize: ${6:'Standard_DS2_v2'}
        osType: 'Linux'
        mode: 'System'
      }
    ]
    linuxProfile: {
      adminUsername: ${7:'adminUserName'}
      ssh: {
        publicKeys: [
          {
            keyData: ${8:'UPDATEME'}
          }
        ]
      }
    }
  }
}
