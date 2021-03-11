param managedInstanceName string
param adminLogin string

@secure()
param adminPassword string

param location string = resourceGroup().location
param virtualNetworkName string = 'vnet-01'
param virtualNetworkPrefix string = '10.0.0.0/16'
param subnetName string = 'subnet-01'
param subnetPrefix string = '10.0.0.0/24'

@allowed([
  'GP_Gen5'
  'BC_Gen5'
])
param skuName string = 'GP_Gen5'

@allowed([
  8
  16
  24
  32
  40
  64
  80
])
param vCores int = 8

@minValue(32)
@maxValue(8192)
param storageSizeInGB int = 256

var networkSecurityGroupName = '${managedInstanceName}-nsg'
var routeTableName = '${managedInstanceName}-routetable'

resource networkSecurityGroup 'Microsoft.Network/networkSecurityGroups@2020-06-01' = {
  name: networkSecurityGroupName
  location: location
  properties: {
    securityRules: [
      {
        name: 'allow_tds_inbound'
        properties: {
          description: 'Allow access to data'
          protocol: 'Tcp'
          sourcePortRange: '*'
          destinationPortRange: '1433'
          sourceAddressPrefix: 'VirtualNetwork'
          destinationAddressPrefix: '*'
          access: 'Allow'
          priority: 1000
          direction: 'Inbound'
        }
      }
      {
        name: 'allow_redirect_inbound'
        properties: {
          description: 'Allow inbound redirect traffic to Managed Instance inside the virtual network'
          protocol: 'Tcp'
          sourcePortRange: '*'
          destinationPortRange: '11000-11999'
          sourceAddressPrefix: 'VirtualNetwork'
          destinationAddressPrefix: '*'
          access: 'Allow'
          priority: 1100
          direction: 'Inbound'
        }
      }

      {
        name: 'deny_all_inbound'
        properties: {
          description: 'Deny all other inbound traffic'
          protocol: '*'
          sourcePortRange: '*'
          destinationPortRange: '*'
          sourceAddressPrefix: '*'
          destinationAddressPrefix: '*'
          access: 'Deny'
          priority: 4096
          direction: 'Inbound'
        }
      }
      {
        name: 'deny_all_outbound'
        properties: {
          description: 'Deny all other outbound traffic'
          protocol: '*'
          sourcePortRange: '*'
          destinationPortRange: '*'
          sourceAddressPrefix: '*'
          destinationAddressPrefix: '*'
          access: 'Deny'
          priority: 4096
          direction: 'Outbound'
        }
      }
    ]
  }
}

resource routeTable 'Microsoft.Network/routeTables@2020-06-01' = {
  name: routeTableName
  location: location
  properties: {
    disableBgpRoutePropagation: false
  }
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
  }
}

resource subnet 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
  name: subnetName
  properties: {
    addressPrefix: subnetPrefix
    routeTable: {
      id: routeTable.id
    }
    networkSecurityGroup: {
      id: networkSecurityGroup.id
    }
    delegations: [
      {
        name: 'managedInstanceDelegation'
        properties: {
          serviceName: 'Microsoft.Sql/managedInstances'
        }
      }
    ]
  }
}

resource managedInstance 'Microsoft.Sql/managedInstances@2020-02-02-preview' = {
  name: managedInstanceName
  location: location
  sku: {
    name: skuName
  }
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    administratorLogin: adminLogin
    administratorLoginPassword: adminPassword
    subnetId: subnet.id
    storageSizeInGB: storageSizeInGB
    vCores: vCores
    licenseType: 'LicenseIncluded'
  }
}
