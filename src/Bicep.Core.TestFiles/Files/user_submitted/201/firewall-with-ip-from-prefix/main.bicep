@description('Specifies the Azure location where the key vault should be created.')
param location string = resourceGroup().location

@description('Specifies the name of the VNet.')
param vnetname string = '${location}-azfw-sample-vnet'

@description('Specifies the address prefix to use for the VNet.')
param vnetaddressprefix string = '10.0.0.0/24'

@description('Specifies the address prefix to use for the AzureFirewallSubnet')
param firewallsubnetprefix string = '10.0.0.0/26'

@allowed([
  28
  29
  30
  31
])
@description('Specifies the size of the Public IP Prefix')
param ipprefixlength int = 31

var firewallname = '${vnetname}-fw'
var publicipname = '${vnetname}-pip'
var ipprefixname = '${vnetname}-ipprefix'

resource vnet 'Microsoft.Network/virtualNetworks@2020-05-01' = {
  name: vnetname
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        vnetaddressprefix
      ]
    }
    subnets: [
      {
        name: 'AzureFirewallSubnet'
        properties: {
          addressPrefix: firewallsubnetprefix
        }
      }
    ]
  }
}

resource ipprefix 'Microsoft.Network/publicIPPrefixes@2020-05-01' = {
  name: ipprefixname
  location: location
  sku: {
    name: 'Standard'
  }
  properties: {
    prefixLength: ipprefixlength
    publicIPAddressVersion: 'IPv4'
    ipTags: []
  }
}

resource publicip 'Microsoft.Network/publicIPAddresses@2020-05-01' = {
  name: publicipname
  location: location
  sku: {
    name: 'Standard'
  }
  properties: {
    publicIPAllocationMethod: 'Static'
    publicIPPrefix: {
      id: ipprefix.id
    }
  }
}

resource firewall 'Microsoft.Network/azureFirewalls@2020-05-01' = {
  name: firewallname
  location: location
  properties: {
    threatIntelMode: 'Alert'
    ipConfigurations: [
      {
        name: '${firewallname}-vnetIpconf'
        properties: {
          subnet: {
            id: '${vnet.id}/subnets/AzureFirewallSubnet'
          }
          publicIPAddress: {
            id: publicip.id
          }
        }
      }
    ]
  }
}
