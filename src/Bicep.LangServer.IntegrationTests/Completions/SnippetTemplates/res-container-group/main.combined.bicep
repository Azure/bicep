resource containerGroup 'Microsoft.ContainerInstance/containerGroups@2021-03-01' = {
  name: 'containerGroup'
  location: resourceGroup().location
  properties: {
    containers: [
      {
        name: 'containerName'
        properties: {
          image: 'containerImage'
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

