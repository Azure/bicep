// Container Group
resource /*${1:containerGroup}*/containerGroup 'Microsoft.ContainerInstance/containerGroups@2021-03-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: {
    containers: [
      {
        name: /*${4:'containername'}*/'containername'
        properties: {
          image: /*${5:'mcr.microsoft.com/azuredocs/aci-helloworld:latest'}*/'mcr.microsoft.com/azuredocs/aci-helloworld:latest'
          ports: [
            {
              port: /*${6:80}*/80
            }
          ]
          resources: {
            requests: {
              cpu: /*${7:1}*/1
              memoryInGB: /*${8:4}*/4
            }
          }
        }
      }
    ]
    restartPolicy: /*${9|'OnFailure','Always','Never'|}*/'OnFailure'
    osType: /*${10|'Linux','Windows'|}*/'Linux'
    ipAddress: {
      type: 'Public'
      ports: [
        {
          protocol: /*${11|'TCP','UDP'|}*/'TCP'
          port: /*${12:80}*/80
        }
      ]
    }
  }
}
