param suffix string = '001'
param owner string = 'alex'
param costCenter string = '12345'
param addressPrefix string = '10.0.0.0/15'

var vnetName = 'vnet-${suffix}'

resource vnet 'Microsoft.Network/virtualNetworks@2018-10-01' = {
  name: vnetName
  location: resourceGroup().location
  tags: {
    Owner: owner
    CostCenter: costCenter
  }
  properties: {
    addressSpace: {
      addressPrefixes: [
        addressPrefix
      ]
    }
    enableVmProtection: false
    enableDdosProtection: false
    subnets: [
      {
        name: 'subnet001'
        properties: {
          addressPrefix: '10.0.0.0/24'
        }
      }
      {
        name: 'subnet002'
        properties: {
          addressPrefix: '10.0.1.0/24'
        }
      }
    ]
  }
}

resource subnet3 'Microsoft.Network/virtualNetworks/subnets@2018-10-01' = {
  name: '${vnetName}/subnet003'
  properties: {
    addressPrefix: '10.0.3.0/24'
  }
}

output stateOfVnet string = subnet3.properties.provisioningState
output vnetType string = subnet3.type
