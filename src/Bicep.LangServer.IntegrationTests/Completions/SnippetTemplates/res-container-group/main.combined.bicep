resource containerGroup 'Microsoft.ContainerInstance/containerGroups@2019-12-01' = {
  name: 'testContainerGroup'
  location: resourceGroup().location
  properties: {
    containers: [
      {
        name: 'testContainerName'
        properties: {
          image: 'testContainerImage'
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
