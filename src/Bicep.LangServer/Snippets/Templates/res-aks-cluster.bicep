// Kubernetes Service Cluster
resource aksCluster 'Microsoft.ContainerService/managedClusters@2021-03-01' = {
  name: ${1:'aksCluster'}
  location: resourceGroup().location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    kubernetesVersion: '${2|1.19.7,1.19.6,1.18.14,1.18.10,1.17.16,1.17.13|}'
    dnsPrefix: ${3:'dnsprefix'}
    enableRBAC: true
    agentPoolProfiles: [
      {
        name: 'agentpool'
        count: ${4:3}
        vmSize: ${5:'Standard_DS2_v2'}
        osType: 'Linux'
        mode: 'System'
      }
    ]
    linuxProfile: {
      adminUsername: ${6:'adminUserName'}
      ssh: {
        publicKeys: [
          {
            keyData: ${7:'keyData'}
          }
        ]
      }
    }
  }
}