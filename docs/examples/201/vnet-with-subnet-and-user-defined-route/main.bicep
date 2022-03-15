// Define Networking parameters
param vnetSuffix string = '001'
param vnetaddressPrefix string = '10.0.0.0/15'
param subnetaddressPrefix string = '10.0.0.0/24'
param subnetName string = 'demoSubNet'
param dnsServer string = '10.0.0.4'
param createUserDefinedRoutes bool = true
param udrName string = 'demoUserDefinedRoute'
param udrRouteName string = 'demoRoute'
param addressPrefix string = '0.0.0.0/24'
param nextHopType string = 'VirtualAppliance'
param nextHopIpAddress string = '10.10.3.4'

var vnetName = 'vnet-${vnetSuffix}'

//Create User Defined Route Acc
resource udr 'Microsoft.Network/routeTables@2020-06-01' = if (createUserDefinedRoutes) {
  name: udrName
  location: resourceGroup().location
  properties: {
    routes: [
      {
        name: udrRouteName
        properties: {
          addressPrefix: addressPrefix
          nextHopType: nextHopType
          nextHopIpAddress: nextHopIpAddress
        }
      }
    ]
    disableBgpRoutePropagation: false
  }
}

//Create Vnet and Subnet
resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: vnetName
  location: resourceGroup().location
  properties: {
    addressSpace: {
      addressPrefixes: [
        vnetaddressPrefix
      ]
    }
    dhcpOptions: {
      dnsServers: [
        dnsServer
      ]
    }
    subnets: [
      {
        name: subnetName
        properties: {
          addressPrefix: subnetaddressPrefix
          routeTable: {
            id: udr.id
          }
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
    ]
  }
}
