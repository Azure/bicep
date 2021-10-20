// $1 = containerGroup
// $2 = 'name'
// $3 = 'containername'
// $4 = 'mcr.microsoft.com/azuredocs/aci-helloworld:latest'
// $5 = 80
// $6 = 1
// $7 = 4
// $8 = 'OnFailure'
// $9 = 'Linux'
// $10 = 'TCP'
// $11 = 80

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

