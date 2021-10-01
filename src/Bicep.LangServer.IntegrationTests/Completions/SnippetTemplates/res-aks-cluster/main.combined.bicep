// $1 = aksCluster
// $2 = 'name'
// $3 = 1.19.7
// $4 = 'dnsPrefix'
// $5 = 3
// $6 = 'Standard_DS2_v2'
// $7 = 'adminUsername'
// $8 = 'REQUIRED'

resource aksCluster 'Microsoft.ContainerService/managedClusters@2021-03-01' = {
  name: 'name'
  location: resourceGroup().location
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
      adminUsername: 'adminUsername'
//@[21:36) [adminusername-should-not-be-literal (Warning)] When setting an adminUserName property, don't use a literal value. Found literal string value "adminUsername" (CodeDescription: bicep core(https://aka.ms/bicep/linter/adminusername-should-not-be-literal)) |'adminUsername'|
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

