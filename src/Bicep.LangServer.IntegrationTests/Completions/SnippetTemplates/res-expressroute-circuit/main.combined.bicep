// $1 = expressRouteCircuit
// $2 = 'name'
// $3 = location
// $4 = 'MeteredData'
// $5 = 'Local'
// $6 = 'Local_MeteredData'
// $7 = 50
// $8 = 'Amsterdam'
// $9 = 'Telia Carrier'
// $10 = false

param location string

resource expressRouteCircuit 'Microsoft.Network/expressRouteCircuits@2020-11-01' = {
  name: 'name'
  location: location
  sku:{
    family: 'MeteredData'
    tier: 'Local'
    name: 'Local_MeteredData'
  }
  properties:{
    serviceProviderProperties:{
      bandwidthInMbps: 50
        peeringLocation: 'Amsterdam'
      serviceProviderName: 'Telia Carrier'
    }
    allowClassicOperations:false
  }
}
// Insert snippet here

