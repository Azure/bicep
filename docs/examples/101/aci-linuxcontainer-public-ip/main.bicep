param name string
param image string = 'mcr.microsoft.com/azuredocs/aci-helloworld'
param port int = 80
param cpuCores int {
  default: 1
}
param memoryinGb int {
  default: 2
}
param restartPolicy string {
  default: 'Always'
  allowed: [
    'Always'
    'Never'
    'OnFailure'
  ]
}
param location string = resourceGroup().location

resource containerGroup 'Microsoft.ContainerInstance/containerGroups@2019-12-01' = {
  name: name
  location: location
  properties: {
    containers: [
      {
        name: name
        properties: {
          image: image
          ports: [
            {
              port: port
              protocol: 'TCP'
            }
          ]
          resources: {
            requests: {
              cpu: cpuCores
              memoryInGB: memoryinGb
            }
          }
        }
      }
    ]
    osType: 'Linux'
    restartPolicy: restartPolicy
    ipAddress: {
      type: 'Public'
      ports: [
        {
          port: port
          protocol: 'TCP'
        }
      ]
    }
  }
}

output containerIPv4Address string = containerGroup.properties.ipAddress.ip
