param nsgName string = 'databricks-nsg'
param vnetName string = 'databricks-vnet'
param workspaceName string
param privateSubnetName string = 'private-subnet'
param publicSubnetName string = 'public-subnet'
param pricingTier string {
  default: 'premium'
  allowed: [
    'standard'
    'premium'
  ]
}
param location string = resourceGroup().location
param vnetCidr string = '10.179.0.0/16'
param privateSubnetCidr string = '10.179.0.0/18'
param publicSubnetCidr string = '10.179.64.0/18'

var managedResourceGroupName = 'databricks-rg-${workspaceName}-${uniqueString(workspaceName, resourceGroup().id)}'
var managedResourceGroupId = '${subscription().id}/resourceGroups/${managedResourceGroupName}'

var publicSubnet = {
  name: publicSubnetName
  properties: {
    addressPrefix: publicSubnetCidr
    networkSecurityGroup: {
      id: nsg.id
    }
    delegations: [
      {
        name: 'databricks-del-public'
        properties: {
          serviceName: 'Microsoft.Databricks/workspaces'
        }
      }
    ]
  }
}

var privateSubnet = {
  name: privateSubnetName
  properties: {
    addressPrefix: privateSubnetCidr
    networkSecurityGroup: {
      id: nsg.id
    }
    delegations: [
      {
        name: 'databricks-del-private'
        properties: {
          serviceName: 'Microsoft.Databricks/workspaces'
        }
      }
    ]
  }
}

resource nsg 'Microsoft.Network/networkSecurityGroups@2019-06-01' = {
  name: nsgName
  location: location
}

resource vnet 'Microsoft.Network/virtualNetworks@2019-06-01' = {
  name: vnetName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        vnetCidr
      ]
    }
    subnets: [
      publicSubnet
      privateSubnet
    ]
  }
}

resource ws 'Microsoft.Databricks/workspaces@2018-04-01' = {
  name: workspaceName
  location: location
  sku: {
    name: pricingTier
  }
  properties: {
    // TODO: improve once we have scoping functions
    managedResourceGroupId: managedResourceGroupId
    parameters: {
      customVirtualNetworkId: {
        value: vnet.id
      }
      customPublicSubnetName: {
        value: publicSubnetName
      }
      customPrivateSubnetName: {
        value: privateSubnetName
      }
    }
  }
}