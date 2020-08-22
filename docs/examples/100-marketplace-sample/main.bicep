param location string = resourceGroup().location

param vmName string = 'nix'

param adminUsername string

param adminPasswordOrKey string {
    secure: true
    minLength: 12
    metadata: {
        description: 'test'
    }
}
param authenticationType string {
    default: 'password'
    allowedValues: [
        'sshPublicKey'
        'password'
    ]
}
param vmSize string = 'Standard_A2_v2'

param storageNewOrExisting string {
    default: 'new'
    allowedValues: [
        'new'
        'existing'
    ]
}
param storageAccountName string = concat('storage', uniqueString(resourceGroup().id))

param storageAccountType string {
    default: 'Standard_LRS'
    allowedValues: [
        'Standard_LRS'
        'Standard_GRS'
        'Standard_RA-GRS'
        'Premium_LRS'
    ]
}
param storageAccountResourceGroupName string = resourceGroup().name

param virtualNetworkNewOrExisting string {
    default: 'new'
    allowedValues: [
        'new'
        'existing'
    ]
}
param virtualNetworkName string = 'VirtualNetwork'

param addressPrefixes array = [ 
    '10.0.0.0/16'
]

param subnetName string = 'default'

param subnetPrefix string = '10.0.0.0/24'

param virtualNetworkResourceGroupName string = resourceGroup().name

param publicIpNewOrExisting string {
    default: 'new'
    allowedValues: [
        'new'
        'existing'
        'none'
    ]
}

param publicIpName string = 'publicIp'

param publicIpDns string = '${toLower(vmName)}-${uniqueString(resourceGroup().id)}'

param publicIpResourceGroupName string = resourceGroup().name

param publicIpAllocationMethod string {
    default: 'Dynamic'
    allowedValues: [
        'Dynamic'
        'Static'
        ''
    ]
}

param publicIpSku string {
    default: 'Basic'
    allowedValues: [
        'Standard'
        'Basic'
        ''
    ]
}

param _artifactsLocation string = deployment().properties.templateLink.uniqueString

param _artifactsLocationSasToken string {
    secure: true
    default: ''
}

// TODO: need a better way to deal with existing resources using symbolic name - this works but only on a per property basis and all the references need to account for it
// will need to refactor templates when we come up with that better way
// currently this template will not work with existing resources (since some references would use an anti-pattern for bicep)
var storageId = storageNewOrExisting =~ 'new' ? storage.id : resourceId(storageAccountResourceGroupName, 'Microsoft.Storage/storageAccounts', storageAccountName)
var nsgName = '${virtualNetworkName}-nsg'
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
    id: publicIp.id
}
var fileToBeCopied = 'FileToBeCopied.txt'
var scriptFolder = 'scripts'
var scriptFileName = 'copyfilefromazure.sh'
var scriptArgs = ' -a ${uri(_artifactsLocation, '.')} -t "${_artifactsLocationSasToken}" -p ${scriptFolder} -f ${fileToBeCopied}'

resource pid 'Microsoft.Resources/deployments@2020-05-01' = {
    name: 'pid-00000000-0000-0000-0000-000000000000'
    properties: {
        mode: 'incremental'
        template: {
            '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
            contentVersion: '1.0.0.0'
            resources: [
            ]
        }
    }
}

// todo this must be conditional
resource storage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: storageAccountName
    location: location
    kind: 'Storage'
    sku: {
        name: storageAccountType
    }

}

resource nsg 'Microsoft.network/networkSecurityGroups@2020-05-01' = {
    name: nsgName
    location: location
    properties: {
        securityRules: [
            {
                name: 'ssh'
                properties: {
                    description: 'Allow RDP'
                    protocol: 'Tcp'
                    sourcePortRange: '*'
                    destinationPortRange: 22
                    sourceAddressPrefix: '*'
                    destinationAddressPrefix: '*'
                    access: 'Allow'
                    priority: 200
                    direction: 'Inbound'
                }
            }
        ]
    }
}

// todo conditional
resource publicIp 'Microsoft.Network/publicIPAddresses@2020-05-01' = {
    name: publicIpName
    location: location
    sku: {
        name: publicIpSku
    }
    properties: {
        publicIpAllocationMethod: publicIpAllocationMethod
        dnsSettings: {
            domainNameLabel: publicIpDns
        }
    }
}

//todo conditional
resource virtualNetwork 'Microsoft.Network/virtualNetworks@2020-05-01' = {
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
                }
            }
        ]
    }
}

resource nic 'Microsoft.Network/networkInterfaces@2020-05-01' = {
    name: nicName
    location: location
    properties: {
        ipConfigurations: [
            {
                name: 'ipConfig1'
                properties: {
                    privateIPAllocationMethod: 'Dynamic'
                    subnet: {
                        id: '${virtualNetwork.id}/subnets/${subnetName}'
                    }
                    //TODO in main.json this isn't compiled correctly due to #316 to work around can replace with:
                    //"publicIPAddress": "[if(not(equals(parameters('publicIpNewOrExisting'), 'none')), json(concat('{\"id\":\"', resourceId('Microsoft.Network/publicIPAddresses', parameters('publicIpName')), '\"}' )), json('null'))]"
                    publicIPAddress: publicIpNewOrExisting != 'none' ? publicIpAddressId : null
                }
            }
        ]
        networkSecurityGroup: {
            id: nsg.id
        }
    }
}

resource vm 'Microsoft.Compute/virtualMachines@2019-12-01' = {
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
            linuxConfiguration: authenticationType =~ 'password' ? null : linuxConfiguration
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
                storageUri: storage.properties.primaryEndpoints.blob
            }
        }
    }
}

resource configScript 'Microsoft.Compute/virtualMachines/extensions@2019-12-01' = {
    name: '${vmName}/configScript'
    location: location
    tags: {
        dependsOnTODO: vm.id  //TODO explicit dependsOn
    }
    properties: {
        publisher: 'Microsoft.Azure.Extensions'
        type: 'CustomScript'
        typeHandlerVersion: '2.0'
        autoUpgradeMinorVersion: true
        settings: {
            fileUris: [
                '${_artifactsLocation}${scriptFolder}/${scriptFileName}${_artifactsLocationSasToken}'
            ]
        }
        protectedSettings: {
            commandToExecute: 'bash ${scriptFileName} ${scriptArgs}'
        }
    }
}

output sshcommand string = publicIpNewOrExisting =~ 'none' ? 'no public ip, vnet access only' : 'ssh ${adminUsername}@${publicIp.properties.dnsSettings.fqdn}'
