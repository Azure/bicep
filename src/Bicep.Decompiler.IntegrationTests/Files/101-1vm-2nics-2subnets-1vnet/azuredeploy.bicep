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

var virtualMachineName = 'VM-MultiNic'
var nic1 = 'nic-1'
var nic2 = 'nic-2'
var virtualNetworkName = 'virtualNetwork'
var subnet1Name = 'subnet-1'
var subnet2Name = 'subnet-2'
var publicIPAddressName = 'publicIp'
var subnet1Ref = resourceId('Microsoft.Network/virtualNetworks/subnets', virtualNetworkName, subnet1Name)
var subnet2Ref = resourceId('Microsoft.Network/virtualNetworks/subnets', virtualNetworkName, subnet2Name)
var diagStorageAccountName = 'diags${uniqueString(resourceGroup().id)}'
var networkSecurityGroupName = 'NSG'
var networkSecurityGroupName2 = '${subnet2Name}-nsg'

resource virtualMachineName_resource 'Microsoft.Compute/virtualMachines@2019-12-01' = {
  name: virtualMachineName
  location: location
  properties: {
    osProfile: {
      computerName: virtualMachineName
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
          id: nic1_resource.id
        }
        {
          properties: {
            primary: false
          }
          id: nic2_resource.id
        }
      ]
    }
    diagnosticsProfile: {
      bootDiagnostics: {
        enabled: true
        storageUri: reference(diagStorageAccountName_resource.id, '2019-06-01').primaryEndpoints.blob
      }
    }
  }
  dependsOn: [
    nic1
//@[4:8) [BCP034 (Warning)] The enclosing array expected an item of type "resource | module", but the provided item was of type "'nic-1'". |nic1|
    nic2
//@[4:8) [BCP034 (Warning)] The enclosing array expected an item of type "resource | module", but the provided item was of type "'nic-2'". |nic2|
    diagStorageAccountName
//@[4:26) [BCP034 (Warning)] The enclosing array expected an item of type "resource | module", but the provided item was of type "string". |diagStorageAccountName|
  ]
}

resource diagStorageAccountName_resource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: diagStorageAccountName
  location: location
  sku: {
    name: storageAccountType
  }
  kind: 'StorageV2'
}

resource networkSecurityGroupName2_resource 'Microsoft.Network/networkSecurityGroups@2020-05-01' = {
  name: networkSecurityGroupName2
  location: location
}

resource virtualNetworkName_resource 'Microsoft.Network/virtualNetworks@2020-05-01' = {
  name: virtualNetworkName
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
            id: networkSecurityGroupName2_resource.id
          }
        }
      }
    ]
  }
  dependsOn: [
    networkSecurityGroupName2_resource
  ]
}

resource nic1_resource 'Microsoft.Network/networkInterfaces@2020-05-01' = {
  name: nic1
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
            id: publicIpAddressName_resource.id
          }
        }
      }
    ]
    networkSecurityGroup: {
      id: networkSecurityGroupName_resource.id
    }
  }
  dependsOn: [
    publicIPAddressName
//@[4:23) [BCP034 (Warning)] The enclosing array expected an item of type "resource | module", but the provided item was of type "'publicIp'". |publicIPAddressName|
    networkSecurityGroupName
//@[4:28) [BCP034 (Warning)] The enclosing array expected an item of type "resource | module", but the provided item was of type "'NSG'". |networkSecurityGroupName|
    virtualNetworkName
//@[4:22) [BCP034 (Warning)] The enclosing array expected an item of type "resource | module", but the provided item was of type "'virtualNetwork'". |virtualNetworkName|
  ]
}

resource nic2_resource 'Microsoft.Network/networkInterfaces@2020-05-01' = {
  name: nic2
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
    virtualNetworkName_resource
  ]
}

resource publicIpAddressName_resource 'Microsoft.Network/publicIPAddresses@2020-05-01' = {
  name: publicIPAddressName
  location: location
  properties: {
    publicIPAllocationMethod: 'Dynamic'
  }
}

resource networkSecurityGroupName_resource 'Microsoft.Network/networkSecurityGroups@2020-05-01' = {
  name: networkSecurityGroupName
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
