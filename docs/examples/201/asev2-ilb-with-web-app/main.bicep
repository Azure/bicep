param location string = resourceGroup().location
param aseName string

@allowed([
  'None'
  'Publishing'
  'Web'
  'Web,Publishing'
])
param internalLoadBalancingMode string = 'Web,Publishing'

param dnsSuffix string
param websiteName string
param appServicePlanName string
param numberOfWorkers int = 1

@allowed([
  '1'
  '2'
  '3'
])
param workerPool string = '1'

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: 'vnet-01'
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        '10.0.0.0/16'
      ]
    }
  }
}
resource subnet 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
  name: '${virtualNetwork.name}/subnet-01'
  properties: {
    addressPrefix: '10.0.1.0/24'
  }
}
resource hostingEnvironment 'Microsoft.Web/hostingEnvironments@2020-06-01' = {
  name: aseName
  location: location
  kind: 'ASEV2'
  properties: {
    name: aseName
    location: location
    ipsslAddressCount: 0
    internalLoadBalancingMode: internalLoadBalancingMode
    dnsSuffix: dnsSuffix
    virtualNetwork: {
      id: virtualNetwork.id
      subnet: subnet.name
    }
    workerPools: []
  }
}
resource serverFarm 'Microsoft.Web/serverfarms@2020-06-01' = {
  name: appServicePlanName
  location: location
  properties: {
    hostingEnvironmentProfile: {
      id: hostingEnvironment.id
    }
  }
  sku: {
    name: 'I${workerPool}'
    tier: 'Isolated'
    size: 'I${workerPool}'
    family: 'I'
    capacity: numberOfWorkers
  }
}
resource website 'Microsoft.Web/sites@2020-06-01' = {
  name: websiteName
  location: location
  properties: {
    serverFarmId: serverFarm.id
    hostingEnvironmentProfile: {
      id: hostingEnvironment.id
    }
  }
}
