param ddosProtectionPlanName string
param virtualNetworkName string
param location string = resourceGroup().location
param virtualNetworkPrefix string = '172.17.0.0/16'
param subnetPrefix string = '172.17.0.0/24'
param ddosProtectionPlanEnabled bool = true

resource ddosProtectionPlan 'Microsoft.Network/ddosProtectionPlans@2020-06-01' = {
  name: ddosProtectionPlanName
  location: location
}

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: virtualNetworkName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        virtualNetworkPrefix
      ]
    }
    subnets: [
      {
        name: 'default'
        properties: {
          addressPrefix: subnetPrefix
        }
      }
    ]
    enableDdosProtection: ddosProtectionPlanEnabled
    ddosProtectionPlan: {
      id: ddosProtectionPlan.id
    }
  }
}
