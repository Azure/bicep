param vmName string {
    default: 'simpleLinuxVM'
    metadata: {
        description: 'The name of you Virtual Machine.'
    }
}

param adminUsername string {
    metadata: {
        description: 'Username for the Virtual Machine.'
    }
}

param authenticationType string {
    default: 'password'
    allowedValues: [
        'sshPublicKey'
        'password'
    ]
    metadata: {
        description: 'Type of authentication to use on the Virtual Machine. SSH key is recommended.'
    }
}

param adminPasswordOrKey string {
    metadata: {
        description: 'SSH Key or password for the Virtual Machine. SSH key is recommended.'
    }
}

param dnsLabelPrefix string {
    default: toLower('simplelinuxvm-${uniqueString(resourceGroup().id)}')
    metadata: {
        description: 'Unique DNS Name for the Public IP used to access the Virtual Machine.'
    }
}

param ubuntuOSVersion string {
    default: '18.04-LTS'
    allowedValues: [
        '12.04.5-LTS'
        '14.04.5-LTS'
        '16.04.0-LTS'
        '18.04-LTS'
    ]
    metadata: {
        description: 'The Ubuntu version for the VM. This will pick a fully patched image of this given Ubuntu version.'
    }
}

param location string {
    default: resourceGroup().location
    metadata: {
        description: 'Location for all resources.'
    }
}

param vmSize string {
    default: 'Standard_B2s'
    metadata: {
        description: 'The size of the VM'
    }
}

param virtualNetworkName string {
    default: 'vNet'
    metadata: {
        'description': 'Name of the VNET'
    }
}

param subnetName string {
    default: 'Subnet'
    metadata: {
        description: 'Name of the subnet in the virtual network'
    }
}

param networkSecurityGroupName string {
    default: 'SecGroupNet'
    metadata: {
        description: 'Name of the Network Security Group'
    }
}

var publicIPAddressName = '${vmName}PublicIP'
var networkInterfaceName = '${vmName}NetInt'
var subnetRef = resourceId('Microsoft.Network/virtualNetworks/subnets', virtualNetworkName, subnetName)
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
    tags: {
        // Workaround for dependsOn.
        dep0: nsg.name
        dep1: vnet.name
        dep2: publicIP.name
    }
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
                        id: publicIP.id
                    }
                }
            }
        ]
        networkSecurityGroup: {
            id: nsg.id
        }
    }
}

resource nsg 'Microsoft.Network/networkSecurityGroups@2019-02-01' = {
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

resource vnet 'Microsoft.Network/virtualNetworks@2019-04-01' = {
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

resource publicIP 'Microsoft.Network/publicIPAddresses@2019-02-01' = {
    name: publicIPAddressName
    location: location
    properties: {
        publicIpAllocationMethod: 'Dynamic'
        publicIPAddressVersion: 'IPv4'
        dnsSettings: {
            domainNameLabel: dnsLabelPrefix
        }
        idleTimeoutInMinutes: 4
    }
    sku: {
        name: 'Basic'
        tier: 'Regional'
    }
}

resource vm 'Microsoft.Compute/virtualMachines@2019-03-01' = {
    name: vmName
    location: location
    tags: {
        dep0: nic.name
    }
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

output administratorUsername string = adminUsername
output hostname string = publicIP.properties.dnsSettings.fqdn
output sshCommand string = 'ssh${adminUsername}@${publicIP.properties.dnsSettings.fqdn}'
