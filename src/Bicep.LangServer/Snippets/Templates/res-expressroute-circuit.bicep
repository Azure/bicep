// ExpressRoute Circuit
resource /*${1:expressRouteCircuit}*/expressRouteCircuit 'Microsoft.Network/expressRouteCircuits@2020-11-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  sku:{
    family: /*${3|'MeteredData','UnlimitedData'|}*/'MeteredData'
    tier: /*${4|'Local','Standard','Premium'|}*/'Local'
    name: /*${5|'Local_MeteredData','Standard_MeteredData','Premium_MeteredData','Basic_UnlimitedData','Local_UnlimitedData','Standard_UnlimitedData','Premium_UnlimitedData'|}*/'Local_MeteredData'
  }
  properties:{
    serviceProviderProperties:{
      bandwidthInMbps: /*${6|50,100,200,500,1000,2000,5000,10000|}*/50
        peeringLocation: /*${7:'Amsterdam'}*/'Amsterdam'
      serviceProviderName: /*${8:'Telia Carrier'}*/'Telia Carrier'
    }
    allowClassicOperations:/*${9|true,false|}*/true
  }
}
