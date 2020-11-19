param virtualMachineSize string {
  metadata: {
    description: 'Virtual machine size (has to be at least the size of Standard_A3 to support 2 NICs)'
  }
  default: 'Standard_DS1_v2'
}
param adminUsername string {
  metadata: {
    description: 'Default Admin username'
  }
}
param adminPassword string {
  metadata: {
    description: 'Default Admin password'
  }
  secure: true
}
param storageAccountType string {
  allowed: [
    'Standard_LRS'
    'Premium_LRS'
  ]
  metadata: {
    description: 'Storage Account type for the VM and VM diagnostic storage'
  }
  default: 'Standard_LRS'
}
param location string {
  metadata: {
    description: 'Location for all resources.'
  }
  default: resourceGroup().location
}

var virtualMachineName_var = 'VM-MultiNic'
var nic1_var = 'nic-1'
var nic2_var = 'nic-2'
var virtualNetworkName_var = 'virtualNetwork'
var subnet1Name = 'subnet-1'
var subnet2Name = 'subnet-2'
var publicIPAddressName_var = 'publicIp'
var subnet1Ref = resourceId('Microsoft.Network/virtualNetworks/subnets', virtualNetworkName_var, subnet1Name)
var subnet2Ref = resourceId('Microsoft.Network/virtualNetworks/subnets', virtualNetworkName_var, subnet2Name)
var diagStorageAccountName_var = 'diags${uniqueString(resourceGroup().id)}'
var networkSecurityGroupName_var = 'NSG'
var networkSecurityGroupName2_var = '${subnet2Name}-nsg'

resource virtualMachineName 'Microsoft.Compute/virtualMachines@2019-12-01' = {
  name: virtualMachineName_var
  location: location
  properties: {
    osProfile: {
      computerName: virtualMachineName_var
      adminUsername: adminUsername
      adminPassword: adminPassword
      windowsConfiguration: {
        provisionVmAgent: 'true'
//@[8:24) [BCP089 (Warning)] The property "provisionVmAgent" is not allowed on objects of type "WindowsConfiguration". Did you mean "provisionVMAgent"? |provisionVmAgent|
      }
    }
    hardwareProfile: {
      vmSize: virtualMachineSize
    }
    storageProfile: {
      imageReference: {
        publisher: 'MicrosoftWindowsServer'
        offer: 'WindowsServer'
        sku: '2019-Datacenter'
        version: 'latest'
      }
      osDisk: {
        createOption: 'FromImage'
      }
    }
    networkProfile: {
      networkInterfaces: [
        {
          properties: {
            primary: true
          }
          id: nic1.id
        }
        {
          properties: {
            primary: false
          }
          id: nic2.id
        }
      ]
    }
    diagnosticsProfile: {
      bootDiagnostics: {
        enabled: true
        storageUri: reference(diagStorageAccountName.id, '2019-06-01').primaryEndpoints.blob
      }
    }
  }
  dependsOn: [
    nic1
    nic2
    diagStorageAccountName
  ]
}

resource diagStorageAccountName 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: diagStorageAccountName_var
  location: location
  sku: {
    name: storageAccountType
  }
  kind: 'StorageV2'
}

resource networkSecurityGroupName2 'Microsoft.Network/networkSecurityGroups@2020-05-01' = {
  name: networkSecurityGroupName2_var
  location: location
}

resource virtualNetworkName 'Microsoft.Network/virtualNetworks@2020-05-01' = {
  name: virtualNetworkName_var
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
        }
      }
      {
        name: subnet2Name
        properties: {
          addressPrefix: '10.0.1.0/24'
          networkSecurityGroup: {
            id: networkSecurityGroupName2.id
          }
        }
      }
    ]
  }
  dependsOn: [
    networkSecurityGroupName2
  ]
}

resource nic1 'Microsoft.Network/networkInterfaces@2020-05-01' = {
  name: nic1_var
  location: location
  properties: {
    ipConfigurations: [
      {
        name: 'ipconfig1'
        properties: {
          subnet: {
            id: subnet1Ref
          }
          privateIPAllocationMethod: 'Dynamic'
          publicIpAddress: {
//@[10:25) [BCP089 (Warning)] The property "publicIpAddress" is not allowed on objects of type "NetworkInterfaceIPConfigurationPropertiesFormat". Did you mean "publicIPAddress"? |publicIpAddress|
            id: publicIpAddressName.id
          }
        }
      }
    ]
    networkSecurityGroup: {
      id: networkSecurityGroupName.id
    }
  }
  dependsOn: [
    publicIpAddressName
    networkSecurityGroupName
    virtualNetworkName
  ]
}

resource nic2 'Microsoft.Network/networkInterfaces@2020-05-01' = {
  name: nic2_var
  location: location
  properties: {
    ipConfigurations: [
      {
        name: 'ipconfig1'
        properties: {
          subnet: {
            id: subnet2Ref
          }
          privateIPAllocationMethod: 'Dynamic'
        }
      }
    ]
  }
  dependsOn: [
    virtualNetworkName
  ]
}

resource publicIpAddressName 'Microsoft.Network/publicIPAddresses@2020-05-01' = {
  name: publicIPAddressName_var
  location: location
  properties: {
    publicIPAllocationMethod: 'Dynamic'
  }
}

resource networkSecurityGroupName 'Microsoft.Network/networkSecurityGroups@2020-05-01' = {
  name: networkSecurityGroupName_var
  location: location
  properties: {
    securityRules: [
      {
        name: 'default-allow-rdp'
        properties: {
          priority: 1000
          sourceAddressPrefix: '*'
          protocol: 'Tcp'
          destinationPortRange: '3389'
          access: 'Allow'
          direction: 'Inbound'
          sourcePortRange: '*'
          destinationAddressPrefix: '*'
        }
      }
    ]
  }
}
