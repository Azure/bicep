// ExpressRoute Circuit
resource ${1:expressRouteCircuit} 'Microsoft.Network/expressRouteCircuits@2020-11-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  sku:{
    family: ${3|'MeteredData','UnlimitedData'|}
    tier: ${4|'Local','Standard','Premium'|}
    name: ${5|'Local_MeteredData','Standard_MeteredData','Premium_MeteredData','Basic_UnlimitedData','Local_UnlimitedData','Standard_UnlimitedData','Premium_UnlimitedData'|}
  }
  properties:{
    serviceProviderProperties:{
      bandwidthInMbps: ${6|50,100,200,500,1000,2000,5000,10000|}
        peeringLocation: ${7:'Amsterdam'}
      serviceProviderName: ${8:'Telia Carrier'}
    }
    allowClassicOperations:${9|true,false|}
  }
}
