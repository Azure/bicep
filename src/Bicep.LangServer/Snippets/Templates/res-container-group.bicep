// Container Group
resource containerGroup 'Microsoft.ContainerInstance/containerGroups@2018-10-01' = {
  name: '${1:containerGroup}'
  location: resourceGroup().location
  properties: {
    containers: [
      {
        name: '${2:containerName}'
        properties: {
          image: '${3:containerImage}'
          ports: [
            {
              port: '${4:80}'
            }
          ]
          resources: {
            requests: {
              cpu: '${5:1}'
              memoryInGB: '${6:4}'
            }
          }
        }
      }
    ]
    osType: '${7|Linux,Windows|}'
    ipAddress: {
      type: 'Public'
      ports: [
        {
          protocol: '${8|TCP,UDP|}'
          port: '${9:80}'
        }
      ]
    }
  }
}