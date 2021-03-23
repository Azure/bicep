// Kubernetes Service Cluster
resource aksCluster 'Microsoft.ContainerService/managedClusters@2020-02-01' = {
  name: '${1:aksCluster}'
  location: resourceGroup().location
  properties: {
    kubernetesVersion: '${2|1.15.7,1.15.5,1.14.8|}'
    dnsPrefix: '${3:dnsprefix}'
    agentPoolProfiles: [
      {
        name: 'agentpool'
        count: ${4:2}
        vmSize: '${5:Standard_A1}'
        osType: 'Linux'
      }
    ]
    linuxProfile: {
      adminUsername: '${7:adminUserName}'
      ssh: {
        publicKeys: [
          {
            keyData: '${8:keyData}'
          }
        ]
      }
    }
    servicePrincipalProfile: {
      clientId: '${9:servicePrincipalAppId}'
      secret: '${10:servicePrincipalAppPassword}'
    }
  }
}