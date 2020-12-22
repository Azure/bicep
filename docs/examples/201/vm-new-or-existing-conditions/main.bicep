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
param createNewStorageAccount bool {
  metadata: {
    description: 'Determines whether or not a new storage account should be provisioned.'
  }
  default: true
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
param createNewVnet bool {
  metadata: {
    description: 'Determines whether or not a new virtual network should be provisioned.'
  }
  default: true
}
param vnetName string {
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
param vnetResourceGroupName string {
  metadata: {
    description: 'Name of the resource group for the existing virtual network'
  }
  default: resourceGroup().name
}
param createNewPublicIP bool {
  metadata: {
    description: 'Determines whether or not a new public ip should be provisioned.'
  }
  default: true
}
param publicIPName string {
  metadata: {
    description: 'Name of the public ip address'
  }
  default: 'PublicIp'
}
param publicIPDns string {
  metadata: {
    description: 'DNS of the public ip address for the VM'
  }
  default: 'linux-vm-${uniqueString(resourceGroup().id)}'
}
param publicIPResourceGroupName string {
  metadata: {
    description: 'Name of the resource group for the public ip address'
  }
  default: resourceGroup().name
}

var storageAccountId = createNewStorageAccount ? storageAccount.id : resourceId(storageAccountResourceGroupName, 'Microsoft.Storage/storageAccounts/', storageAccountName)
var subnetId = createNewVnet ? subnet.id : resourceId(vnetResourceGroupName, 'Microsoft.Network/virtualNetworks/subnets', vnetName, subnetName)
var publicIPId = createNewPublicIP ? publicIP.id : resourceId(publicIPResourceGroupName, 'Microsoft.Network/publicIPAddresses', publicIPName)

resource storageAccount 'Microsoft.Storage/storageAccounts@2017-06-01' = if (createNewStorageAccount) {
  name: storageAccountName
  location: location
  kind: 'Storage'
  sku: {
    name: storageAccountType
  }
}

resource publicIP 'Microsoft.Network/publicIPAddresses@2017-09-01' = if (createNewPublicIP) {
  name: publicIPName
  location: location
  properties: {
    publicIPAllocationMethod: 'Dynamic'
    dnsSettings: {
      domainNameLabel: publicIPDns
    }
  }
}

resource nsg 'Microsoft.Network/networkSecurityGroups@2019-08-01' = if (createNewVnet) {
  name: 'default-NSG'
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

resource vnet 'Microsoft.Network/virtualNetworks@2017-09-01' = if (createNewVnet) {
  name: vnetName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: addressPrefixes
    }
  }
}

resource subnet 'Microsoft.Network/virtualNetworks/subnets@2017-09-01' = if (createNewVnet) {
  name: '${vnet.name}/${subnetName}'
  properties: {
    addressPrefix: subnetPrefix
    networkSecurityGroup: {
      id: nsg.id
    }
  }
}

resource nic 'Microsoft.Network/networkInterfaces@2017-09-01' = {
  name: '${vmName}-nic'
  location: location
  properties: {
    ipConfigurations: [
      {
        name: 'ipconfig1'
        properties: {
          privateIPAllocationMethod: 'Dynamic'
          subnet: {
            id: subnetId
          }
          publicIPAddress: {
            id: publicIPId
          }
        }
      }
    ]
  }
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
      linuxConfiguration: any(authenticationType != 'password' ? {
        disablePasswordAuthentication: true
        ssh: {
          publicKeys: [
            {
              path: '/home/${adminUsername}/.ssh/authorized_keys'
              keyData: adminPasswordOrKey
            }
          ]
        }
      } : null)
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
        storageUri: reference(storageAccountId).primaryEndpoints.blob
      }
    }
  }
}
