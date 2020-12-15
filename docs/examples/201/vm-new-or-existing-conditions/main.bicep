param location string {
  metadata: {
    description: 'Location to for the resources.'
  }
  default: resourceGroup().location
}
param vmName string {
  metadata: {
    description: 'Name for the Virtual Machine.'
  }
  default: 'linux-vm'
}
param adminUsername string {
  metadata: {
    description: 'User name for the Virtual Machine.'
  }
}
param authenticationType string {
  allowed: [
    'password'
    'sshPublicKey'
  ]
  metadata: {
    description: 'Type of authentication to use on the Virtual Machine.'
  }
  default: 'sshPublicKey'
}
param adminPasswordOrKey string {
  metadata: {
    description: 'Password or ssh key for the Virtual Machine.'
  }
  secure: true
}
param vmSize string {
  metadata: {
    description: 'Size for the Virtual Machine.'
  }
  default: 'Standard_A2_v2'
}
param storageNewOrExisting string {
  metadata: {
    description: 'Determines whether or not a new storage account should be provisioned.'
  }
  default: 'new'
}
param storageAccountName string {
  metadata: {
    description: 'Name of the storage account'
  }
  default: 'storage${uniqueString(resourceGroup().id)}'
}
param storageAccountType string {
  metadata: {
    description: 'Storage account type'
  }
  default: 'Standard_LRS'
}
param storageAccountResourceGroupName string {
  metadata: {
    description: 'Name of the resource group for the existing storage account'
  }
  default: resourceGroup().name
}
param virtualNetworkNewOrExisting string {
  metadata: {
    description: 'Determines whether or not a new virtual network should be provisioned.'
  }
  default: 'new'
}
param virtualNetworkName string {
  metadata: {
    description: 'Name of the virtual network'
  }
  default: 'VirtualNetwork'
}
param addressPrefixes array {
  metadata: {
    description: 'Address prefix of the virtual network'
  }
  default: [
    '10.0.0.0/16'
  ]
}
param subnetName string {
  metadata: {
    description: 'Name of the subnet'
  }
  default: 'default'
}
param subnetPrefix string {
  metadata: {
    description: 'Subnet prefix of the virtual network'
  }
  default: '10.0.0.0/24'
}
param virtualNetworkResourceGroupName string {
  metadata: {
    description: 'Name of the resource group for the existing virtual network'
  }
  default: resourceGroup().name
}
param publicIpNewOrExisting string {
  metadata: {
    description: 'Determines whether or not a new public ip should be provisioned.'
  }
  default: 'new'
}
param publicIpName string {
  metadata: {
    description: 'Name of the public ip address'
  }
  default: 'PublicIp'
}
param publicIpDns string {
  metadata: {
    description: 'DNS of the public ip address for the VM'
  }
  default: 'linux-vm-${uniqueString(resourceGroup().id)}'
}
param publicIpResourceGroupName string {
  metadata: {
    description: 'Name of the resource group for the public ip address'
  }
  default: resourceGroup().name
}

var nicName = '${vmName}-nic'
var linuxConfiguration = {
  disablePasswordAuthentication: true
  ssh: {
    publicKeys: [
      {
        path: '/home/${adminUsername}/.ssh/authorized_keys'
        keyData: adminPasswordOrKey
      }
    ]
  }
}
var publicIpAddressId = {
  id: resourceId(publicIpResourceGroupName, 'Microsoft.Network/publicIPAddresses', publicIpName)
}
var networkSecurityGroupName = 'default-NSG'

resource storageAccount 'Microsoft.Storage/storageAccounts@2017-06-01' = if (storageNewOrExisting == 'new') {
  name: storageAccountName
  location: location
  kind: 'Storage'
  sku: {
    name: storageAccountType
  }
}

resource publicIp 'Microsoft.Network/publicIPAddresses@2017-09-01' = if (publicIpNewOrExisting == 'new') {
  name: publicIpName
  location: location
  properties: {
    publicIPAllocationMethod: 'Dynamic'
    dnsSettings: {
      domainNameLabel: publicIpDns
    }
  }
}

resource nsg 'Microsoft.Network/networkSecurityGroups@2019-08-01' = if (virtualNetworkNewOrExisting == 'new') {
  name: networkSecurityGroupName
  location: location
  properties: {
    securityRules: [
      {
        name: 'default-allow-22'
        properties: {
          priority: 1000
          access: 'Allow'
          direction: 'Inbound'
          destinationPortRange: '22'
          protocol: 'Tcp'
          sourceAddressPrefix: '*'
          sourcePortRange: '*'
          destinationAddressPrefix: '*'
        }
      }
    ]
  }
}

resource vnet 'Microsoft.Network/virtualNetworks@2017-09-01' = if (virtualNetworkNewOrExisting == 'new') {
  name: virtualNetworkName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: addressPrefixes
    }
    subnets: [
      {
        name: subnetName
        properties: {
          addressPrefix: subnetPrefix
          networkSecurityGroup: {
            id: nsg.id
          }
        }
      }
    ]
  }
}

resource nic 'Microsoft.Network/networkInterfaces@2017-09-01' = {
  name: nicName
  location: location
  properties: {
    ipConfigurations: [
      {
        name: 'ipconfig1'
        properties: {
          privateIPAllocationMethod: 'Dynamic'
          subnet: {
            id: resourceId(virtualNetworkResourceGroupName, 'Microsoft.Network/virtualNetworks/subnets/', virtualNetworkName, subnetName)
          }
          publicIPAddress: any(!(publicIpNewOrExisting == 'none') ? publicIpAddressId : null)
        }
      }
    ]
  }
  dependsOn: [
    publicIp
    vnet
  ]
}

resource vm 'Microsoft.Compute/virtualMachines@2017-03-30' = {
  name: vmName
  location: location
  properties: {
    hardwareProfile: {
      vmSize: vmSize
    }
    osProfile: {
      computerName: vmName
      adminUsername: adminUsername
      adminPassword: adminPasswordOrKey
      linuxConfiguration: any((authenticationType == 'password') ? null : linuxConfiguration)
    }
    storageProfile: {
      imageReference: {
        publisher: 'Canonical'
        offer: 'UbuntuServer'
        sku: '16.04-LTS'
        version: 'latest'
      }
      osDisk: {
        caching: 'ReadWrite'
        createOption: 'FromImage'
      }
    }
    networkProfile: {
      networkInterfaces: [
        {
          id: nic.id
        }
      ]
    }
    diagnosticsProfile: {
      bootDiagnostics: {
        enabled: true
        storageUri: reference(resourceId(storageAccountResourceGroupName, 'Microsoft.Storage/storageAccounts/', storageAccountName), '2017-06-01').primaryEndpoints.blob
      }
    }
  }
  dependsOn: [
    storageAccount
  ]
}
