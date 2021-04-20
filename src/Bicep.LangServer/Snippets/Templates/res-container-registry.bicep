﻿// Container Registry
resource ${1:containerRegistry} 'Microsoft.ContainerRegistry/registries@2019-05-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  sku: {
    name: ${3|'Classic','Basic','Standard','Premium'|}
  }
  properties: {
    adminUserEnabled: ${4|true,false|}
  }
}
