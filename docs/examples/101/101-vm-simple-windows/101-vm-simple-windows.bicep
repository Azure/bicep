param adminUserName string
param adminPassword string {
    secure:true
}
param dnsLabelPrefix string
param windowsOSVersion string {
    default :'2016-Datacenter'
    allowedValues : [
        '2008-R2-SP1'
        '2012-Datacenter'
        '2012-R2-Datacenter'
        '2016-Nano-Server'
        '2016-Datacenter-with-Containers'
        '2016-Datacenter'
        '2019-Datacenter'
      ]
      metadata: {
        'description': 'The Windows version for the VM. This will pick a fully patched image of this given Windows version.' 
      }
}
param vmSize string {
    default: 'Standard_D2_v3'
    metadata: {
      description: 'Size of the virtual machine.'
    }
}

param location string {
    default: resourceGroup().location
    metadata: {
        description: 'location for all resources'
    }
}

var  storageAccountName = concat(uniqueString(resourceGroup().id), 'sawinvm')
var nicName =  'myVMNic'
var addressPrefix =  '10.0.0.0/16'
var subnetName = 'Subnet'
var subnetPrefix = '10.0.0.0/24'
var publicIPAddressName =  'myPublicIP'
var vmName = 'SimpleWinVM'
var virtualNetworkName =  'MyVNET'
var subnetRef = '${vn.id}/subnets/${subnetName}'
var networkSecurityGroupName =  'default-NSG'

resource stg  'Microsoft.Storage/storageAccounts@2018-11-01' = {
    name: storageAccountName
    location: location
    sku: {
       name: 'Standard_LRS' 
    }
    kind: 'Storage'
}

resource pip 'Microsoft.Network/publicIPAddresses@2018-11-01' = {
  name: publicIPAddressName
  location: location
  properties: {
    publicIPAllocationMethod: 'Dynamic'
    dnsSettings: {
        domainNameLabel: dnsLabelPrefix
    }
  }
}

resource sg 'Microsoft.Network/networkSecurityGroups@2019-08-01' = {
  name: networkSecurityGroupName
  location: location
  properties: {
      securityRules: [
        {
            name: 'default-allow-3389'
            'properties': {
              priority:  1000
              access:  'Allow'
              direction:  'Inbound'
              destinationPortRange: '3389'
              protocol:  'Tcp'
              sourcePortRange:  '*'
              sourceAddressPrefix:  '*'
              destinationAddressPrefix:  '*'
            }
          }
      ]
  }
}

resource vn 'Microsoft.Network/virtualNetworks@2018-11-01' = {
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
          addressPrefix: subnetPrefix
          networkSecurityGroup: {
            id: sg.id
          }
        }
      }
    ] 
  }
}

resource nInter 'Microsoft.Network/networkInterfaces@2018-11-01' = {
    name: nicName
    location: location

    properties: {
        ipConfigurations: [
          {
            name: 'ipconfig1'
            properties: {
              privateIPAllocationMethod: 'Dynamic'
              publicIPAddress: {
                id: pip.id
              }
              subnet: {
                id: subnetRef
              }
            }
          }
        ]
      }
    }

resource VM 'Microsoft.Compute/virtualMachines@2018-10-01' = {
  name: vmName
  location: location
  tags: {
    dependsOn: stg.id
  }
  properties: {
    hardwareProfile: {
        vmSize: vmSize
      }
      osProfile: {
        computerName: vmName
        adminUsername: adminUserName
        adminPassword: adminPassword
      }
      storageProfile: {
        imageReference: {
          publisher: 'MicrosoftWindowsServer'
          offer: 'WindowsServer'
          sku: windowsOSVersion
          version: 'latest'
        }
        osDisk: {
          createOption: 'FromImage'
        }
        dataDisks: [
          {
            diskSizeGB: 1023
            lun: 0
            createOption: 'Empty'
          }
        ]
      }
      networkProfile: {
        networkInterfaces: [
          {
            id: nInter.id
          }
        ]
      }
      diagnosticsProfile: {
        bootDiagnostics: {
          enabled: true
          storageUri: reference(stg.id).primaryEndpoints.blob
        }
      }
  }
}

output hostname string = reference(publicIPAddressName).dnsSettings.fqdn
