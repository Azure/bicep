// ExpressRoute Gateway
resource /*${1:expressRouteGateways}*/expressRouteGateways 'Microsoft.Network/expressRouteGateways@2021-02-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  properties: {
    virtualHub: {
      id: /*${3:'virtualHub.id'}*/'virtualHub.id'
    }
    autoScaleConfiguration: {
      bounds: {
        min: /*${4|1,2,3,4,5,6,7,8|}*/1
        max: /*${5|1,2,3,4,5,6,7,8|}*/2
      }
    }
  }
}
