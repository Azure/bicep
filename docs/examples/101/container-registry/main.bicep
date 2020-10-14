// params
param acrName string {
  default : 'acr001${uniqueString(resourceGroup().id)}' // must be globally unique
  metadata: {
    description: 'Specifies the name of the azure container registry.'
  }
  minLength: 5
  maxLength: 50
}
param acrAdminUserEnabled bool {
  default : false
  metadata: {
    description: 'Enable admin user that have push / pull permission to the registry.'
  }
}
param location string {
  default: resourceGroup().location
  metadata: {
    description: 'Specifies the Azure location where the key vault should be created.'
  }
}
param acrSku string {
    default: 'Basic'
    allowed: [
        'Basic'
        'Standard'
        'Premium'
    ]
    metadata : {
        'description': 'Tier of your Azure Container Registry.'
    }
}

// azure container registry
resource acr 'Microsoft.ContainerRegistry/registries@2019-12-01-preview' = {
  name: acrName
  location: location
  sku: {
      name: acrSku
  }
  properties: {
      adminUserEnabled: acrAdminUserEnabled
  }
}

output acrLoginServer string = acr.properties.loginServer
