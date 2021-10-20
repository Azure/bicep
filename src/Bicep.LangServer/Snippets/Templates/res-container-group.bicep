// Container Group
resource /*${1:containerGroup}*/containerGroup 'Microsoft.ContainerInstance/containerGroups@2021-03-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  properties: {
    containers: [
      {
        name: /*${3:'containername'}*/'containername'
        properties: {
          image: /*${4:'mcr.microsoft.com/azuredocs/aci-helloworld:latest'}*/'mcr.microsoft.com/azuredocs/aci-helloworld:latest'
          ports: [
            {
              port: /*${5:80}*/80
            }
          ]
          resources: {
            requests: {
              cpu: /*${6:1}*/1
              memoryInGB: /*${7:4}*/4
            }
          }
        }
      }
    ]
    restartPolicy: /*${8|'OnFailure','Always','Never'|}*/'OnFailure'
    osType: /*${9|'Linux','Windows'|}*/'Linux'
    ipAddress: {
      type: 'Public'
      ports: [
        {
          protocol: /*${10|'TCP','UDP'|}*/'TCP'
          port: /*${11:80}*/80
        }
      ]
    }
  }
}
