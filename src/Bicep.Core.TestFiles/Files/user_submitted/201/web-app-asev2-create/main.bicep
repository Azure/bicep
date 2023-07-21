param location string = resourceGroup().location
param virtualNetworkName string = 'vnet-01'
param subnetName string = 'subnet-01'
param aseName string = uniqueString(resourceGroup().name)

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: virtualNetworkName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        '10.0.0.0/16'
      ]
    }
  }
}

resource subNet 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
  name: subnetName
  properties: {
    addressPrefix: '10.0.1.0/25'
  }
}

resource ase 'Microsoft.Web/hostingEnvironments@2020-06-01' = {
  name: aseName
  location: location
  kind: 'ASEV2'
  properties: {
    location: location
    name: aseName
    workerPools: []
    virtualNetwork: {
      id: subNet.id
    }
  }
}
