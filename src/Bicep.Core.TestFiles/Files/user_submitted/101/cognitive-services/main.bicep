param serviceName string = 'cognitive-${uniqueString(resourceGroup().id)}'
param location string = resourceGroup().location
param sku string = 'S0'

resource cognitiveService 'Microsoft.CognitiveServices/accounts@2017-04-18' = {
  name: serviceName
  location: location
  sku: {
    name: sku
  }
  kind: 'CognitiveServices'
}
