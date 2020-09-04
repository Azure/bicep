// The name of your Virtual Machine.
param vmName string = 'linuxvm'

// Username for the Virtual Machine.
param adminUsername string

// Type of authentication to use on the Virtual Machine. SSH key is recommended.
param authenticationType string {
    default: 'password'
    allowed: [
        'sshPublicKey'
        'password'
    ]
}

// SSH Key or password for the Virtual Machine. SSH key is recommended.
param adminPasswordOrKey string

// Unique DNS Name for the Public IP used to access the Virtual Machine.
param dnsLabelPrefix string = toLower('${vmName}-${uniqueString(resourceGroup().id)}')

// The Ubuntu version for the VM. This will pick a fully patched image of this given Ubuntu version.
param ubuntuOSVersion string {
    default: '18.04-LTS'
    allowed: [
        '12.04.5-LTS'
        '14.04.5-LTS'
        '16.04.0-LTS'
        '18.04-LTS'
    ]
}

// Location for all resources.
param location string = resourceGroup().location

// The size of the VM.
param vmSize string = 'Standard_B2s'

// Name of the VNET.
param virtualNetworkName string = 'vNet'

// Name of the subnet in the virtual network.
param subnetName string = 'Subnet'

// Name of the Network Security Group.
param networkSecurityGroupName string = 'SecGroupNet'

param zone string {
  default: '1'
  allowed: [
    '1'
    '2'
    '3'
  ]
}

var publicIPAddressName = '${vmName}PublicIP'
var networkInterfaceName = '${vmName}NetInt'
var subnetRef = '${vnet.id}/subnets/${subnetName}'
var osDiskType = 'Standard_LRS'
var subnetAddressPrefix = '10.1.0.0/24'
var addressPrefix = '10.1.0.0/16'
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

resource nic 'Microsoft.Network/networkInterfaces@2018-10-01' = {
  name: networkInterfaceName
  location: location
  properties: {
    ipConfigurations: [
      {
        name: 'ipconfig1'
        properties: {
          subnet: {
            id: subnetRef
          }
          privateIPAllocationMethod: 'Dynamic'
          publicIpAddress: {
            id: pubIp.id
          }
        }
      }
    ]
    networkSecurityGroup: {
        id: nsg.id
    }
  }
}

resource nsg 'Microsoft.Network/networkSecurityGroups@2020-05-01' = {
    name: networkSecurityGroupName
    location: location
    properties: {
        securityRules: [
            {
                name: 'SSH'
                properties: {
                    priority: 1000
                    protocol: 'TCP'
                    access: 'Allow'
                    direction: 'Inbound'
                    sourceAddressPrefix: '*'
                    sourcePortRange: '*'
                    destinationAddressPrefix: '*'
                    destinationPortRange: '22'
                }
            }
        ]
    }
}

resource vnet 'Microsoft.Network/virtualNetworks@2020-05-01' = {
    name: virtualNetworkName
    location: location
    properties: {
        addressSpace: {
            addressPrefixes: [
                addressPrefix
            ]
        }
        subnets: [
            {
                name: subnetName
                properties: {
                    addressPrefix: subnetAddressPrefix
                    privateEndpointNetworkPolicies: 'Enabled'
                    privateLinkServiceNetworkPolicies: 'Enabled'
                }
            }
        ]
    }
}

resource vm 'Microsoft.Compute/virtualMachines@2019-03-01' = {
  name: vmName
  location: location
  zones: [
    zone
  ]
  properties: {
    hardwareProfile: {
      vmSize: vmSize
    }
    storageProfile: {
      osDisk: {
        createOption: 'fromImage'
        managedDisk: {
          storageAccountType: osDiskType
        }
      }
      imageReference: {
        publisher: 'Canonical'
        offer: 'UbuntuServer'
        sku: ubuntuOSVersion
        version: 'latest'
      }
    }
    networkProfile: {
      networkInterfaces: [
        {
          id: nic.id
        }
      ]
    }
    osProfile: {
      computerName: vmName
      adminUsername: adminUsername
      adminPassword: adminPasswordOrKey
      linuxConfiguration: authenticationType == 'password' ? null : linuxConfiguration
    }
  }
}

var zones = length(vm.zones) > 0 ? vm.zones : []

// resource pubipv4 'Microsoft.Network/publicIpAddresses@2020-05-01' = {
//   name: '${vm.name}-pip'
//   zones: vm.properties.network.enabledPublicIpZone ? zones : null
//   location: location
//   sku: {
//     name: 'Basic'
//     tier: 'Regional'
//   }
//   properties: {
//     publicIpAllocationMethod: 'Dynamic'
//     publicIPAddressVersion: 'IPv4'
//     dnsSettings: {
//       domainNameLabel: dnsLabelPrefix
//     }
//   }
// }

// to repro error - replace pubIp with any other name to break the cyclical dependency
resource pubIp 'Microsoft.Network/publicIPAddresses@2020-05-01' = {
  name: '${vm.name}-pip'
  zones: vm.properties.network.enabledPublicIpZone ? zones : null
  location: location
  sku: {
    name: 'Basic'
    tier: 'Regional'
  }
  properties: {
    publicIpAllocationMethod: 'Dynamic'
    publicIPAddressVersion: 'IPv4'
    dnsSettings: {
      domainNameLabel: dnsLabelPrefix
    }
    idleTimeoutInMinutes: 4
  }
}

output administratorUsername string = adminUsername
output hostname string = pubIp.properties.dnsSettings.fqdn
output sshCommand string = 'ssh${adminUsername}@${pubIp.properties.dnsSettings.fqdn}'
