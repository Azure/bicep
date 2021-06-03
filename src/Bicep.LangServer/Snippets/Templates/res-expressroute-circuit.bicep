// ExpressRoute Circuit

resource expressRouteCircuit 'Microsoft.Network/expressRouteCircuits@2020-11-01' = {
  name: ${1:'name'}
  location: resourceGroup().location
  sku:{
    family: ${2|'MeteredData','UnlimitedData'|}
    tier: ${3|'Local','Standard','Premium'|}
    name: ${4|'Local_MeteredData','Standard_MeteredData','Premium_MeteredData','Basic_UnlimitedData','Local_UnlimitedData','Standard_UnlimitedData','Premium_UnlimitedData'|}
  }
  properties:{
    serviceProviderProperties:{
      bandwidthInMbps: ${5|50,100,200,500,1000,2000,5000,10000|}
        peeringLocation: ${6:'Amsterdam'}
      serviceProviderName: ${7:'Telia Carrier'}
    }
    allowClassicOperations:${8|true,false|}
  }
}
