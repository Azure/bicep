// ExpressRoute Circuit
resource /*${1:expressRouteCircuit}*/expressRouteCircuit 'Microsoft.Network/expressRouteCircuits@2020-11-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  sku:{
    family: /*${4|'MeteredData','UnlimitedData'|}*/'MeteredData'
    tier: /*${5|'Local','Standard','Premium'|}*/'Local'
    name: /*${6|'Local_MeteredData','Standard_MeteredData','Premium_MeteredData','Basic_UnlimitedData','Local_UnlimitedData','Standard_UnlimitedData','Premium_UnlimitedData'|}*/'Local_MeteredData'
  }
  properties:{
    serviceProviderProperties:{
      bandwidthInMbps: /*${7|50,100,200,500,1000,2000,5000,10000|}*/50
        peeringLocation: /*${8:'Amsterdam'}*/'Amsterdam'
      serviceProviderName: /*${9:'Telia Carrier'}*/'Telia Carrier'
    }
    allowClassicOperations:/*${10|true,false|}*/true
  }
}
