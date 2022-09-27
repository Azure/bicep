@description('Admin username for VM')
param adminUsername string

@description('Number of VMs to deploy, limit 398 since there is an 800 resource limit for a single template deployment')
@minValue(2)
@maxValue(398)
param numberOfInstances int = 4

@description('OS Platform for the VM')
@allowed([
  'Ubuntu'
  'Windows'
])
param OS string = 'Ubuntu'

@description('Location for all resources.')
param location string = resourceGroup().location

@description('Type of authentication to use on the Virtual Machine. SSH key is recommended.')
@allowed([
  'sshPublicKey'
  'password'
])
param authenticationType string = 'sshPublicKey'

@description('SSH Key or password for the Virtual Machine. SSH key is recommended.')
@secure()
param adminPasswordOrKey string

@description('description')
param vmSize string = 'Standard_A1_v2'

var virtualNetworkName = 'VNET'
var addressPrefix = '10.0.0.0/16'
var subnet1Name = 'Subnet-1'
var subnet2Name = 'Subnet-2'
var subnet1Prefix = '10.0.0.0/24'
var subnet2Prefix = '10.0.1.0/24'
var subnet1Ref = resourceId('Microsoft.Network/virtualNetworks/subnets', virtualNetworkName, subnet1Name)
var subnet2Ref = resourceId('Microsoft.Network/virtualNetworks/subnets', virtualNetworkName, subnet2Name)
var availabilitySetName = 'AvSet'
var imageReference = {
  Ubuntu: {
    publisher: 'Canonical'
    offer: 'UbuntuServer'
    sku: '18.04-LTS'
    version: 'latest'
  }
  Windows: {
    publisher: 'MicrosoftWindowsServer'
    offer: 'WindowsServer'
    sku: '2019-Datacenter'
    version: 'latest'
  }
}
var networkSecurityGroupName = 'default-NSG'
var nsgOsPort = {
  Ubuntu: '22'
  Windows: '3389'
}
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

resource availabilitySet 'Microsoft.Compute/availabilitySets@2020-06-01' = [for i in range(0, 2): {
  name: '${availabilitySetName}-${i}'
  location: location
  properties: {
    platformFaultDomainCount: 2
    platformUpdateDomainCount: 2
  }
  sku: {
    name: 'Aligned'
  }
}]

resource networkSecurityGroup 'Microsoft.Network/networkSecurityGroups@2020-05-01' = {
  name: networkSecurityGroupName
  location: location
  properties: {
    securityRules: [
      {
        name: 'default-allow-${nsgOsPort[OS]}'
        properties: {
          priority: 1000
          access: 'Allow'
          direction: 'Inbound'
          destinationPortRange: nsgOsPort[OS]
          protocol: 'Tcp'
          sourceAddressPrefix: '*'
          sourcePortRange: '*'
          destinationAddressPrefix: '*'
        }
      }
    ]
  }
}

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2020-05-01' = {
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
        name: subnet1Name
        properties: {
          addressPrefix: subnet1Prefix
          networkSecurityGroup: {
            id: networkSecurityGroup.id
          }
        }
      }
      {
        name: subnet2Name
        properties: {
          addressPrefix: subnet2Prefix
          networkSecurityGroup: {
            id: networkSecurityGroup.id
          }
        }
      }
    ]
  }
}

resource nic 'Microsoft.Network/networkInterfaces@2020-05-01' = [for i in range(0, numberOfInstances): {
  name: 'nic${i}'
  location: location
  properties: {
    ipConfigurations: [
      {
        name: 'ipconfig1'
        properties: {
          privateIPAllocationMethod: 'Dynamic'
          subnet: {
            id: (((i % 2) == 0) ? subnet1Ref : subnet2Ref)
          }
        }
      }
    ]
  }
  dependsOn: [
    virtualNetwork
  ]
}]

resource myvm 'Microsoft.Compute/virtualMachines@2020-06-01' = [for i in range(0, numberOfInstances): {
  name: 'myvm${i}'
  location: location
  properties: {
    availabilitySet: {
      id: resourceId('Microsoft.Compute/availabilitySets', '${availabilitySetName}-${(i % 2)}')
    }
    hardwareProfile: {
      vmSize: vmSize
    }
    osProfile: {
      computerName: 'vm${i}'
      adminUsername: adminUsername
      adminPassword: adminPasswordOrKey
      linuxConfiguration: ((authenticationType == 'password') ? json('null') : linuxConfiguration)
    }
    storageProfile: {
      imageReference: imageReference[OS]
      osDisk: {
        createOption: 'FromImage'
      }
    }
    networkProfile: {
      networkInterfaces: [
        {
          id: resourceId('Microsoft.Network/networkInterfaces', 'nic${i}')
        }
      ]
    }
  }
  dependsOn: [
    resourceId('Microsoft.Network/networkInterfaces', 'nic${i}')
//@[4:64) [BCP034 (Error)] The enclosing array expected an item of type "module[] | (resource | module) | resource[]", but the provided item was of type "string". (CodeDescription: none) |resourceId('Microsoft.Network/networkInterfaces', 'nic${i}')|
    availabilitySet
  ]
}]
