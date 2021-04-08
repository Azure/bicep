// Resource name
param name string
// Resource suffix
param suffix string
// Resource location
param location string = resourceGroup().location

// Unique DNS Name for the Public IP used to access the Virtual Machine.
param publicIpDnsLabel string

// CIDR notation of the Virtual Networks.
param virtualNetworkAddressPrefix string = '10.0.0.0/16'
// CIDR notation of the Virtual Network Subnets.
param virtualNetworkSubnetPrefix string = '10.0.0.0/24'

// Username for the Virtual Machine.
param virtualMachineAdminUsername string

// Password for the Virtual Machine.
@secure()
param virtualMachineAdminPassword string

// Size of the Virtual Machine.
@allowed([
  'Standard_D2s_v3'
  'Standard_D4s_v3'
  'Standard_D8s_v3'
])
param virtualMachineSize string = 'Standard_D8s_v3'

// The publisher of the Virtual Machine.
@allowed([
  'MicrosoftVisualStudio'
  'MicrosoftWindowsDesktop'
])
param virtualMachinePublisher string = 'MicrosoftWindowsDesktop'

// The offer of the Virtual Machine
@allowed([
  'visualstudio2019latest'
  'Windows-10'
])
param virtualMachineOffer string = 'Windows-10'

// The Windows version for the VM. This will pick a fully patched image of this given Windows version.
@allowed([
  'vs-2019-comm-latest-ws2019'
  'vs-2019-ent-latest-ws2019'
  '20h1-pro-g2'
  '20h1-ent-g2'
])
param virtualMachineSku string = '20h1-pro-g2'

// The URI of the PowerShell Custom Script.
param virtualMachineExtensionCustomScriptUri string = 'https://raw.githubusercontent.com/Azure/bicep/main/docs/examples/201/vm-windows-with-custom-script-extension/install.ps1'

var metadata = {
  longName: '{0}-${name}-${(suffix ?? '') == '' ? '' : concat('-', suffix)}'
  shortName: '{0}${replace(name, '-', '')}${(suffix ?? '') == '' ? '' : suffix}'
}

// Stroage Account
var storageAccount = {
  name: replace(metadata.shortName, '{0}', 'st')
  location: location
}

resource st 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: storageAccount.name
  location: storageAccount.location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

// Public IP
var publicIp = {
  name: replace(metadata.longName, '{0}', 'pip')
  location: location
}

resource pip 'Microsoft.Network/publicIPAddresses@2020-06-01' = {
  name: publicIp.name
  location: publicIp.location
  properties: {
    publicIPAllocationMethod: 'Static'
    dnsSettings: {
      domainNameLabel: (publicIpDnsLabel ?? '') == '' ? replace(metadata.shortName, '{0}', 'vm') : publicIpDnsLabel
    }
  }
}

// Network Security Group
var networkSecurityGroup = {
  name: replace(metadata.longName, '{0}', 'nsg')
  location: location
  securityRules: [
    {
      name: 'default-allow-3389'
      properties: {
        priority: 1000
        access: 'Allow'
        direction: 'Inbound'
        protocol: 'TCP'
        sourcePortRange: '*'
        sourceAddressPrefix: '*'
        destinationAddressPrefix: '*'
        destinationPortRange: 3389
      }
    }
  ]
}

resource nsg 'Microsoft.Network/networkSecurityGroups@2020-06-01' = {
  name: networkSecurityGroup.name
  location: networkSecurityGroup.location
  properties: {
    securityRules: networkSecurityGroup.securityRules
  }
}

// Virtual Network
var subnetName = 'default'
var virtualNetwork = {
  name: replace(metadata.longName, '{0}', 'vnet')
  location: location
  addressPrefixes: [
    virtualNetworkAddressPrefix
  ]
  subnets: [
    {
      name: subnetName
      properties: {
        addressPrefix: virtualNetworkSubnetPrefix
        networkSecurityGroup: {
          id: nsg.id
        }
      }
    }
  ]
}

resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: virtualNetwork.name
  location: virtualNetwork.location
  properties: {
    addressSpace: {
      addressPrefixes: virtualNetwork.addressPrefixes
    }
    subnets: virtualNetwork.subnets
  }
}

// Network Interface
var networkInterface = {
  name: replace(metadata.longName, '{0}', 'nic')
  location: location
  ipConfigurations: [
    {
      name: 'ipconfig1'
      properties: {
        privateIPAllocationMethod: 'Dynamic'
        publicIPAddress: {
          id: pip.id
        }
        subnet: {
          id: resourceId('Microsoft.Network/virtualNetworks/subnets', vnet.name, subnetName)
        }
      }
    }
  ]
}

resource nic 'Microsoft.Network/networkInterfaces@2020-06-01' = {
  name: networkInterface.name
  location: networkInterface.location
  properties: {
    ipConfigurations: networkInterface.ipConfigurations
  }
}

// Virtual Machine
var virtualMachine = {
  name: replace(metadata.shortName, '{0}', 'vm')
  location: location

  vmSize: virtualMachineSize
  vmComputerName: replace(metadata.shortName, '{0}', 'vm')
  vmAdminUsername: virtualMachineAdminUsername
  vmAdminPassword: virtualMachineAdminPassword

  vmImagePublisher: virtualMachinePublisher
  vmImageOffer: virtualMachineOffer
  vmImageSku: virtualMachineSku

  vmOSDiskName: replace(metadata.longName, '{0}', 'osdisk')

  vmDataDisks: []

  networkInterfaces: [
    {
      id: nic.id
    }
  ]
}

resource vm 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  name: virtualMachine.name
  location: virtualMachine.location
  properties: {
    hardwareProfile: {
      vmSize: virtualMachine.vmSize
    }
    osProfile: {
      computerName: virtualMachine.vmComputerName
      adminUsername: virtualMachine.vmAdminUsername
      adminPassword: virtualMachine.vmAdminPassword
    }
    storageProfile: {
      imageReference: {
        publisher: virtualMachine.vmImagePublisher
        offer: virtualMachine.vmImageOffer
        sku: virtualMachine.vmImageSku
        version: 'latest'
      }
      osDisk: {
        name: virtualMachine.vmOSDiskName
        caching: 'ReadWrite'
        createOption: 'FromImage'
      }
      dataDisks: virtualMachine.vmDataDisks
    }
    networkProfile: {
      networkInterfaces: virtualMachine.networkInterfaces
    }
    diagnosticsProfile: {
      bootDiagnostics: {
        enabled: true
        storageUri: st.properties.primaryEndpoints.blob
      }
    }
  }
}

// Virtual Machine Extensions - Custom Script
var virtualMachineExtensionCustomScript = {
  name: '${vm.name}/config-app'
  location: location
  fileUris: [
    virtualMachineExtensionCustomScriptUri
  ]
  commandToExecute: 'powershell -ExecutionPolicy Unrestricted -File ./${last(split(virtualMachineExtensionCustomScriptUri, '/'))}'
}

resource vmext 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
  name: virtualMachineExtensionCustomScript.name
  location: virtualMachineExtensionCustomScript.location
  properties: {
    publisher: 'Microsoft.Compute'
    type: 'CustomScriptExtension'
    typeHandlerVersion: '1.10'
    autoUpgradeMinorVersion: true
    settings: {
      fileUris: virtualMachineExtensionCustomScript.fileUris
      commandToExecute: virtualMachineExtensionCustomScript.commandToExecute
    }
    protectedSettings: {}
  }
}
