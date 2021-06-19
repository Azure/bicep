// coverted from: https://github.com/Azure/azure-quickstart-templates/tree/master/201-vm-push-certificate-windows
param location string = resourceGroup().location

param vmName string = 'WindowsVM'
param vmSize string = 'Standard_DS2_v2'
param adminUsername string

@secure()
param adminPassword string

param keyVaultName string
param subId string = subscription().subscriptionId
param rgName string = resourceGroup().name

var kvId = resourceId(subId, rgName, 'Microsoft.KeyVault/vaults', keyVaultName)

param secretUrlWithVersion string // what is the format of this?

var subnet1Name = 'subnet-1'
var vnetName = 'certVnet'
var nsgName = '${subnet1Name}-nsg'

resource pip 'microsoft.network/publicIpAddresses@2020-06-01' = {
  name: 'certPublicIp'
  location: location
  properties: {
    publicIPAllocationMethod: 'Dynamic'
  }
}

resource nsg 'microsoft.network/networkSecurityGroups@2020-08-01' = {
  name: nsgName
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

resource vnet 'microsoft.network/virtualNetworks@2020-06-01' = {
  name: vnetName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        '10.0.0.0/16'
      ]
    }
    subnets: [
      {
        name: subnet1Name
        properties: {
          addressPrefix: '10.0.0.0/24'
          networkSecurityGroup: {
            id: nsg.id
          }
        }
      }
    ]
  }
}

resource nic 'microsoft.network/networkInterfaces@2020-06-01' = {
  name: 'certNic'
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
            id: '${vnet.id}/subnets/${subnet1Name}' // resourceId() would not generate dependsOn correctly
          }
        }
      }
    ]
  }
}

resource vm 'microsoft.compute/virtualMachines@2020-06-01' = {
  name: vmName
  location: location
  properties: {
    hardwareProfile: {
      vmSize: vmSize
    }
    osProfile: {
      computerName: vmName
      adminUsername: adminUsername
      adminPassword: adminPassword
      secrets: [
        {
          sourceVault: {
            id: kvId
          }
          vaultCertificates: [
            {
              certificateUrl: secretUrlWithVersion
              certificateStore: 'My'
            }
          ]
        }
      ]
    }
    storageProfile: {
      imageReference: {
        publisher: 'MicrosoftWindowsServer'
        offer: 'WindowsServer'
        sku: '2019-Datacenter'
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
  }
}
