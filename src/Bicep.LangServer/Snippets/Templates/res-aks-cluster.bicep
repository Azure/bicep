// Kubernetes Service Cluster
resource /*${1:aksCluster}*/aksCluster 'Microsoft.ContainerService/managedClusters@2021-03-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    kubernetesVersion: /*'${3|1.19.7,1.19.6,1.18.14,1.18.10,1.17.16,1.17.13|}'*/'1.19.7'
    dnsPrefix: /*${4:'dnsprefix'}*/'dnsprefix'
    enableRBAC: true
    agentPoolProfiles: [
      {
        name: 'agentpool'
        count: /*${5:3}*/3
        vmSize: /*${6:'Standard_DS2_v2'}*/'Standard_DS2_v2'
        osType: 'Linux'
        mode: 'System'
      }
    ]
    linuxProfile: {
      adminUsername: /*${7:'adminUserName'}*/'adminUserName'
      ssh: {
        publicKeys: [
          {
            keyData: /*${8:'REQUIRED'}*/'REQUIRED'
          }
        ]
      }
    }
  }
}
