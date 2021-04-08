param virtualMachineAdminUserName string = 'azadmin'

@secure()
param virtualMachineAdminPassword string

param virtualMachineNamePrefix string = 'MyVM0'

param virtualMachineCount int = 3

param virtualMachineSize string = 'Standard_DS2_v2'

@allowed([
  'Server2012R2'
  'Server2016'
  'Server2019'
])
param operatingSystem string = 'Server2019'

param availabilitySetName string = 'MyAvailabilitySet'

param dnsPrefixForPublicIP string = uniqueString(resourceGroup().id)

param location string = resourceGroup().location

var myVNETName = 'myVNET'
var myVNETPrefix = '10.0.0.0/16'
var myVNETSubnet1Name = 'Subnet1'
var myVNETSubnet1Prefix = '10.0.0.0/24'
var diagnosticStorageAccountName = 'diagst${uniqueString(resourceGroup().id)}'
var operatingSystemValues = {
  Server2012R2: {
    PublisherValue: 'MicrosoftWindowsServer'
    OfferValue: 'WindowsServer'
    SkuValue: '2012-R2-Datacenter'
  }
  Server2016: {
    PublisherValue: 'MicrosoftWindowsServer'
    OfferValue: 'WindowsServer'
    SkuValue: '2016-Datacenter'
  }
  Server2019: {
    PublisherValue: 'MicrosoftWindowsServer'
    OfferValue: 'WindowsServer'
    SkuValue: '2019-Datacenter'
  }
}
var availabilitySetPlatformFaultDomainCount = 2
var availabilitySetPlatformUpdateDomainCount = 5
var networkSecurityGroupName = 'default-NSG'

resource networkSecurityGroup 'Microsoft.Network/networkSecurityGroups@2020-06-01' = {
  name: networkSecurityGroupName
  location: location
  properties: {
    securityRules: [
      {
        name: 'default-allow-3389'
        properties: {
          priority: 1000
          access: 'Allow'
          direction: 'Inbound'
          destinationPortRange: '3389'
          protocol: 'Tcp'
          sourceAddressPrefix: '*'
          sourcePortRange: '*'
          destinationAddressPrefix: '*'
        }
      }
    ]
  }
}

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: myVNETName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        myVNETPrefix
      ]
    }
  }
}

resource subNet 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
  name: '${myVNETName}/${myVNETSubnet1Name}'
  properties: {
    addressPrefix: myVNETSubnet1Prefix
    networkSecurityGroup: {
      id: networkSecurityGroup.id
    }
  }
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
  name: diagnosticStorageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}

resource availabilitySet 'Microsoft.Compute/availabilitySets@2020-06-01' = {
  name: availabilitySetName
  location: location
  properties: {
    platformFaultDomainCount: availabilitySetPlatformFaultDomainCount
    platformUpdateDomainCount: availabilitySetPlatformUpdateDomainCount
  }
  sku: {
    name: 'Aligned'
  }
}

resource virtualMachines 'Microsoft.Compute/virtualMachines@2020-06-01' = [for i in range(0, virtualMachineCount): {
  name: '${virtualMachineNamePrefix}${i + 1}'
  location: location
  properties: {
    hardwareProfile: {
      vmSize: virtualMachineSize
    }
    storageProfile: {
      imageReference: {
        publisher: operatingSystemValues[operatingSystem].PublisherValue
        offer: operatingSystemValues[operatingSystem].OfferValue
        sku: operatingSystemValues[operatingSystem].SkuValue
        version: 'latest'
      }
      osDisk: {
        name: '${virtualMachineNamePrefix}${i + 1}'
        createOption: 'FromImage'
        managedDisk: {
          storageAccountType: 'Standard_LRS'
        }
        caching: 'ReadWrite'
      }
    }
    osProfile: {
      computerName: '${virtualMachineNamePrefix}${i + 1}'
      adminUsername: virtualMachineAdminUserName
      windowsConfiguration: {
        provisionVMAgent: true
      }
      adminPassword: virtualMachineAdminPassword
    }
    networkProfile: {
      networkInterfaces: [
        {
          id: resourceId('Microsoft.Network/networkInterfaces', '${virtualMachineNamePrefix}${i + 1}-NIC1')
        }
      ]
    }
    availabilitySet: {
      id: availabilitySet.id
    }
    diagnosticsProfile: {
      bootDiagnostics: {
        enabled: true
        storageUri: storageAccount.properties.primaryEndpoints.blob
      }
    }
  }
}]

resource networkInterfaces 'Microsoft.Network/networkInterfaces@2020-06-01' = [for i in range(0, virtualMachineCount): {
  name: '${virtualMachineNamePrefix}${i + 1}-NIC1'
  location: location
  properties: {
    ipConfigurations: [
      {
        name: 'ipconfig1'
        properties: {
          privateIPAllocationMethod: 'Dynamic'
          publicIPAddress: {
            id: resourceId('Microsoft.Network/publicIPAddresses', '${virtualMachineNamePrefix}${i + 1}-PIP1')
          }
          subnet: {
            id: subNet.id
          }
        }
      }
    ]
    enableIPForwarding: false
  }
}]
resource publicIPAddresses 'Microsoft.Network/publicIPAddresses@2020-06-01' = [for i in range(0, virtualMachineCount): {
  name: '${virtualMachineNamePrefix}${i + 1}-PIP1'
  location: location
  properties: {
    publicIPAllocationMethod: 'Dynamic'
    dnsSettings: {
      domainNameLabel: '${dnsPrefixForPublicIP}${i + 1}'
    }
  }
}]
