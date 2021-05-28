//Array Lookup of a complex object
var subnetArray = [
  'GatewaySubnet'
  'AzureBastionSubnet'
  'AzureFirewallSubnet'
  'websubnet'
  'appsubnet'
  'sqlsubnet'
]

//This complex object is used for lookups.
var subnetObj = {
  GatewaySubnet: {
    NSG: false
    RouteTable: true
    AddressSpace: '10.1.0.0/27'
  }
  AzureBastionSubnet: {
    NSG: false
    RouteTable: false
    AddressSpace: '10.1.0.33/27'
  }
  AzureFirewallSubnet: {
    NSG: false
    RouteTable: false
    AddressSpace: '10.1.0.33/27'
  }
  websubnet: {
    NSG: true
    RouteTable: true
    AddressSpace: '10.1.1.0/24'
  }
  appsubnet: {
    NSG: true
    RouteTable: true
    AddressSpace: '10.1.2.0/24'
  }
  sqlsubnet: {
    NSG: true
    RouteTable: true
    AddressSpace: '10.1.3.0/24'
  }
}

output bicepObject array = [for (name, i) in subnetArray: {
  name: subnetArray[i]
  AddressSpace: subnetObj[subnetArray[i]].AddressSpace
  NSG: subnetObj[subnetArray[i]].NSG
  RouteTable: subnetObj[subnetArray[i]].RouteTable
}]
