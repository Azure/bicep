// Container Group
resource containerGroup 'Microsoft.ContainerInstance/containerGroups@2021-03-01' = {
  name: ${1:'containerGroup'}
  location: resourceGroup().location
  properties: {
    containers: [
      {
        name: ${2:'containerName'}
        properties: {
          image: ${3:'containerImage'}
          ports: [
            {
              port: ${4:80}
            }
          ]
          resources: {
            requests: {
              cpu: ${5:1}
              memoryInGB: ${6:4}
            }
          }
        }
      }
    ]
    restartPolicy: '${7|OnFailure,Always,Never|}'
    osType: '${8|Linux,Windows|}'
    ipAddress: {
      type: 'Public'
      ports: [
        {
          protocol: '${9|TCP,UDP|}'
          port: ${10:80}
        }
      ]
    }
  }
}
