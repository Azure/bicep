// $1 = containerGroup
// $2 = 'name'
// $3 = location
// $4 = 'containername'
// $5 = 'mcr.microsoft.com/azuredocs/aci-helloworld:latest'
// $6 = 80
// $7 = 1
// $8 = 4
// $9 = 'OnFailure'
// $10 = 'Linux'
// $11 = 'TCP'
// $12 = 80

param location string

resource containerGroup 'Microsoft.ContainerInstance/containerGroups@2021-03-01' = {
  name: 'name'
  location: location
  properties: {
    containers: [
      {
        name: 'containername'
        properties: {
          image: 'mcr.microsoft.com/azuredocs/aci-helloworld:latest'
          ports: [
            {
              port: 80
            }
          ]
          resources: {
            requests: {
              cpu: 1
              memoryInGB: 4
            }
          }
        }
      }
    ]
    restartPolicy: 'OnFailure'
    osType: 'Linux'
    ipAddress: {
      type: 'Public'
      ports: [
        {
          protocol: 'TCP'
          port: 80
        }
      ]
    }
  }
}
// Insert snippet here

