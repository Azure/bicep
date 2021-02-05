param location string = resourceGroup().location
param fwname string
param fwtype string {
  allowed: [
    'VNet'
    'vWAN'
  ]
  metadata: {
    description: 'Specify if the Azure Firewall should be deployed to VNet or Virtual WAN Hub'
  }
}
param fwpolicyid string {
  metadata: {
    description: 'Resoruce ID to the Firewall Policy to associate with the Azure Firewall'
  }
}
param hubid string {
  default: ''
  metadata: {
    description: 'Virtual Hub Resource ID, used when deploying Azure Firewall to Virtual WAN'
  }
}
param hubpublicipcount int {
  default: 1
  metadata: {
    description: 'Specifies the number of public IPs to allocate to the firewall when deploying Azure Firewall to Virtual WAN'
  }
}
param subnetid string {
  default: ''
  metadata: {
    description: 'AzureFirewallSubnet ID, used when deploying Azure Firewall to Virtual Network'
  }
}
param publicipid string {
  default: ''
  metadata: {
    description: 'Azure Firewall Public IP ID, used when deploying Azure Firewall to Virtual Network'
  }
}

var hubfwproperties = {
  properties: {
    sku: {
      name: 'AZFW_Hub'
      tier: 'Standard'
    }
    virtualHub: {
      id: hubid
    }
    hubIPAddresses: {
      publicIPs: {
        count: hubpublicipcount
      }
    }
    firewallPolicy: {
      id: fwpolicyid
    }
  }
}

var vnetfwproperties = {
  properties: {
    sku: {
      name: 'AZFW_VNet'
      tier: 'Standard'
    }
    ipConfigurations: [
      {
        name: '${fwname}-vnetIPConf'
        properties: {
          subnet: {
            id: subnetid
          }
          publicIPAddress: {
            id: publicipid
          }
        }
      }
    ]
    firewallPolicy: {
      id: fwpolicyid
    }
  }
}

resource firewall 'Microsoft.Network/azureFirewalls@2020-06-01' = {
  name: fwname
  location: location
  properties: fwtype == 'VNet' ? vnetfwproperties.properties : fwtype == 'vWAN' ? hubfwproperties.properties : any(null)
}
