@description('Location to for the resources.')
param location string = resourceGroup().location

@description('Name for the Virtual Machine.')
param vmName string = 'linux-vm'

@description('User name for the Virtual Machine.')
param adminUsername string

@allowed([
  'password'
  'sshPublicKey'
])
@description('Type of authentication to use on the Virtual Machine.')
param authenticationType string = 'sshPublicKey'

@secure()
@description('Password or ssh key for the Virtual Machine.')
param adminPasswordOrKey string

@description('Size for the Virtual Machine.')
param vmSize string = 'Standard_A2_v2'

@description('Determines whether or not a new storage account should be provisioned.')
param createNewStorageAccount bool = true

@description('Name of the storage account')
param storageAccountName string = 'storage${uniqueString(resourceGroup().id)}'

@description('Storage account type')
param storageAccountType string = 'Standard_LRS'

@description('Name of the resource group for the existing storage account')
param storageAccountResourceGroupName string = resourceGroup().name

@description('Determines whether or not a new virtual network should be provisioned.')
param createNewVnet bool = true

@description('Name of the virtual network')
param vnetName string = 'VirtualNetwork'

@description('Address prefix of the virtual network')
param addressPrefixes array = [
  '10.0.0.0/16'
]

@description('Name of the subnet')
param subnetName string = 'default'

@description('Subnet prefix of the virtual network')
param subnetPrefix string = '10.0.0.0/24'

@description('Name of the resource group for the existing virtual network')
param vnetResourceGroupName string = resourceGroup().name

@description('Determines whether or not a new public ip should be provisioned.')
param createNewPublicIP bool = true

@description('Name of the public ip address')
param publicIPName string = 'PublicIp'

@description('DNS of the public ip address for the VM')
param publicIPDns string = 'linux-vm-${uniqueString(resourceGroup().id)}'

@description('Name of the resource group for the public ip address')
param publicIPResourceGroupName string = resourceGroup().name

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
