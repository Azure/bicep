resource cau 'Microsoft.Resources/deployments@2020-06-01' = {
  name: 'pid-xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx'
  properties: {
    mode: 'Incremental'
    template: {
      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
      contentVersion: '1.0.0.0'
      resources: []
    }
  }
}
