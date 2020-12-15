param domainName string = 'aaddscontoso.com'

resource aadds 'Microsoft.AAD/domainServices@2020-01-01' = {
  name: domainName
  location: 'West Europe'
  properties: {
    domainName: domainName
    domainSecuritySettings: {
      ntlmV1: 'Enabled'
      syncKerberosPasswords: 'Enabled'
      syncNtlmPasswords: 'Enabled'
      syncOnPremPasswords: 'Enabled'
      tlsV1: 'Enabled'
    }
    filteredSync: 'Disabled'
    ldapsSettings: {
      externalAccess: 'Disabled'
      ldaps: 'Disabled'
    }
    notificationSettings: {
      notifyDcAdmins: 'Enabled'
      notifyGlobalAdmins: 'Enabled'
      additionalRecipients: []
    }
    replicaSets: [
      {
        subnetId: vnetEUS.properties.subnets[0].id
        location: vnetEUS.location
      }
      {
        subnetId: vnetWEU.properties.subnets[0].id
        location: vnetWEU.location
      }
    ]
    sku: 'Enterprise'
  }
}

resource nsgEUS 'Microsoft.Network/networkSecurityGroups@2020-06-01' = {
  name: 'aadds-eus-nsg'
  location: 'East US'
  properties: {
    securityRules: [
      {
        name: 'AllowSyncWithAzureAD'
        properties: {
          access: 'Allow'
          priority: 101
          direction: 'Inbound'
          protocol: 'Tcp'
          sourceAddressPrefix: 'AzureActiveDirectoryDomainServices'
          sourcePortRange: '*'
          destinationAddressPrefix: '*'
          destinationPortRange: '443'
        }
      }
      {
        name: 'AllowPSRemoting'
        properties: {
          access: 'Allow'
          priority: 301
          direction: 'Inbound'
          protocol: 'Tcp'
          sourceAddressPrefix: 'AzureActiveDirectoryDomainServices'
          sourcePortRange: '*'
          destinationAddressPrefix: '*'
          destinationPortRange: '5986'
        }
      }
      {
        name: 'AllowRD'
        properties: {
          access: 'Allow'
          priority: 201
          direction: 'Inbound'
          protocol: 'Tcp'
          sourceAddressPrefix: 'CorpNetSaw'
          sourcePortRange: '*'
          destinationAddressPrefix: '*'
          destinationPortRange: '3389'
        }
      }
    ]
  }
}

resource vnetEUS 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: 'aadds-eus-vnet'
  location: 'East US'
  properties: {
    addressSpace: {
      addressPrefixes: [
        '10.0.0.0/24'
      ]
    }
    subnets: [
      {
        name: 'aadds-subnet'
        properties: {
          addressPrefix: '10.0.0.0/24'
          networkSecurityGroup: {
            id: nsgEUS.id
          }
        }
      }
    ]
  }
}

resource nsgWEU 'Microsoft.Network/networkSecurityGroups@2020-06-01' = {
  name: 'aadds-weu-nsg'
  location: 'West Europe'
  properties: {
    securityRules: [
      {
        name: 'AllowSyncWithAzureAD'
        properties: {
          access: 'Allow'
          priority: 101
          direction: 'Inbound'
          protocol: 'Tcp'
          sourceAddressPrefix: 'AzureActiveDirectoryDomainServices'
          sourcePortRange: '*'
          destinationAddressPrefix: '*'
          destinationPortRange: '443'
        }
      }
      {
        name: 'AllowPSRemoting'
        properties: {
          access: 'Allow'
          priority: 301
          direction: 'Inbound'
          protocol: 'Tcp'
          sourceAddressPrefix: 'AzureActiveDirectoryDomainServices'
          sourcePortRange: '*'
          destinationAddressPrefix: '*'
          destinationPortRange: '5986'
        }
      }
      {
        name: 'AllowRD'
        properties: {
          access: 'Allow'
          priority: 201
          direction: 'Inbound'
          protocol: 'Tcp'
          sourceAddressPrefix: 'CorpNetSaw'
          sourcePortRange: '*'
          destinationAddressPrefix: '*'
          destinationPortRange: '3389'
        }
      }
    ]
  }
}

resource vnetWEU 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: 'aadds-weu-vnet'
  location: 'West Europe'
  properties: {
    addressSpace: {
      addressPrefixes: [
        '10.1.0.0/24'
      ]
    }
    subnets: [
      {
        name: 'aadds-subnet'
        properties: {
          addressPrefix: '10.1.0.0/24'
          networkSecurityGroup: {
            id: nsgWEU.id
          }
        }
      }
    ]
  }
}

resource peerEUS 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings@2020-06-01' = {
  name: '${vnetEUS.name}/${vnetEUS.name}-peering-${vnetWEU.name}'
  properties: {
    allowVirtualNetworkAccess: true
    allowForwardedTraffic: true
    allowGatewayTransit: false
    useRemoteGateways: false
    remoteVirtualNetwork: {
      id: vnetWEU.id
    }
  }
}

resource peerWEU 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings@2020-06-01' = {
  name: '${vnetWEU.name}/${vnetWEU.name}-peering-${vnetEUS.name}'
  properties: {
    allowVirtualNetworkAccess: true
    allowForwardedTraffic: true
    allowGatewayTransit: false
    useRemoteGateways: false
    remoteVirtualNetwork: {
      id: vnetEUS.id
    }
  }
}
