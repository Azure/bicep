param privateDnsZoneName string = 'contoso.com'
param autoVmRegistration bool = true
param vnetName string = 'Vnet1'
param vnetAddressPrefix string = '10.0.0.0/16'
param subnetName string = 'Subnet1'
param subnetAddressPrefix string = '10.0.1.0/24'
param location string = resourceGroup().location

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: vnetName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        vnetAddressPrefix
      ]
    }
  }
}

resource subnet 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
  name: '${virtualNetwork.name}/${subnetName}'
  properties: {
    addressPrefix: subnetAddressPrefix
    networkSecurityGroup: {
      properties: {
        securityRules: [
          {
            properties: {
              direction: 'Inbound'
              protocol: '*'
              access: 'Allow'
            }
          }
          {
            properties: {
              direction: 'Outbound'
              protocol: '*'
              access: 'Allow'
            }
          }
        ]
      }
    }
  }
}

resource privateDnsZone 'Microsoft.Network/privateDnsZones@2018-09-01' = {
  name: privateDnsZoneName
  location: 'global'
}

resource virtualNetworkLink 'Microsoft.Network/privateDnsZones/virtualNetworkLinks@2018-09-01' = {
  name: '${privateDnsZone.name}/${privateDnsZone.name}-link'
  location: 'global'
  properties: {
    registrationEnabled: autoVmRegistration
    virtualNetwork: {
      id: virtualNetwork.id
    }
  }
}
