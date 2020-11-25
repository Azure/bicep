// Define Networkin parameters
param vnetName string
param vnetaddressPrefix string
param subnetPrefix string
param vnetLocation string = 'westeurope'
param subnetName string = 'WVD'

//Create Vnet and Subnet
resource vnet 'Microsoft.Network/virtualnetworks@2015-05-01-preview' = {
  name: vnetName
  location: vnetLocation
   properties: {
    addressSpace: {
      addressPrefixes: [
        vnetaddressPrefix
      ]
    }
    subnets: [
      {
        name: subnetName
        properties: {
          addressPrefix: subnetPrefix
        }
      }
    ]
  }
}
