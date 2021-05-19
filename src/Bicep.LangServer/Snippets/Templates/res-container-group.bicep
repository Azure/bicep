﻿// Container Group
resource ${1:containerGroup} 'Microsoft.ContainerInstance/containerGroups@2021-03-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  properties: {
    containers: [
      {
        name: ${3:'containername'}
        properties: {
          image: ${4:'mcr.microsoft.com/azuredocs/aci-helloworld:latest'}
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
    restartPolicy: ${8|'OnFailure','Always','Never'|}
    osType: ${9|'Linux','Windows'|}
    ipAddress: {
      type: 'Public'
      ports: [
        {
          protocol: ${10|'TCP','UDP'|}
          port: ${11:80}
        }
      ]
    }
  }
}
