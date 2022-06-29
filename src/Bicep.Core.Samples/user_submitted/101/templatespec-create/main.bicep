param templateSpecName string = 'ManagedDisk'
param templateSpecVersionName string = '0.1'
param location string = resourceGroup().location

resource templateSpec 'Microsoft.Resources/templateSpecs@2019-06-01-preview' = {
  name: templateSpecName
  location: location
  properties: {
    description: 'A basic templateSpec - creates a managed disk.'
    displayName: 'Managed Disk (Standard_LRS)'
  }
}

resource templateSpecVersion 'Microsoft.Resources/templateSpecs/versions@2019-06-01-preview' = {
  name: '${templateSpec.name}/${templateSpecVersionName}'
  location: location
  properties: {
    template: {
      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
      contentVersion: '1.0.0.0'
      parameters: {
        diskName: {
          type: 'string'
        }
        location: {
          type: 'string'
        }
      }
      resources: [
        {
          type: 'Microsoft.Compute/disks'
          name: '[parameters(\'diskName\')]'
          apiVersion: '2020-05-01'
          location: '[parameters(\'location\')]'
          sku: {
            name: 'Standard_LRS'
          }
          properties: {
            creationData: {
              createOption: 'Empty'
            }
            diskSizeGB: 64
          }
        }
      ]
    }
  }
}
