// $1 = expressRouteGateways
// $2 = 'name'
// $3 = 'virtualHub.id'
// $4 = 1
// $5 = 2

param location string

resource expressRouteGateways 'Microsoft.Network/expressRouteGateways@2021-02-01' = {
  name: 'name'
  location: location
  properties: {
    virtualHub: {
      id: 'virtualHub.id'
    }
    autoScaleConfiguration: {
      bounds: {
        min: 1
        max: 2
      }
    }
  }
}
// Insert snippet here

