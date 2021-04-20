// Container Group
resource ${1:'containerGroup'} 'Microsoft.ContainerInstance/containerGroups@2021-03-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  properties: {
    containers: [
      {
        name: ${3:'containerName'}
        properties: {
          image: ${4:'UPDATEME'}
          ports: [
            {
              port: ${5:80}
            }
          ]
          resources: {
            requests: {
              cpu: ${6:1}
              memoryInGB: ${7:4}
            }
          }
        }
      }
    ]
    restartPolicy: '${8|OnFailure,Always,Never|}'
    osType: '${9|Linux,Windows|}'
    ipAddress: {
      type: 'Public'
      ports: [
        {
          protocol: '${10|TCP,UDP|}'
          port: ${10:80}
        }
      ]
    }
  }
}
