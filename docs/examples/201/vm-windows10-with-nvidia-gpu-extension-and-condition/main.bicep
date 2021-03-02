//define parameters
param localAdminName string = 'localadmin'

@secure()
param localAdminPassword string

param vnetName string = 'bicep-demo-vnet'
param vmSku string = 'Standard_NV6'
param vmOS string = '20h2-ent'
param installNVidiaGPUDriver bool = true
param vmName string = 'bicep-demo-vm'

//define variables
var defaultLocation = resourceGroup().location
var defaultVmNicName = '${vmName}-nic'
var vnetConfig = {
  vnetprefix: '10.0.0.0/16'
  subnet: {
    name: 'bicep-demo-subnet'
    subnetPrefix: '10.0.66.0/24'
  }
}
var privateIPAllocationMethod = 'Dynamic'

//Create vnet
resource vnet 'Microsoft.Network/virtualNetworks@2020-05-01' = {
  name: vnetName
  location: defaultLocation
  properties: {
    addressSpace: {
      addressPrefixes: [
        vnetConfig.vnetprefix
      ]
    }
    subnets: [
      {
        name: vnetConfig.subnet.name
        properties: {
          addressPrefix: vnetConfig.subnet.subnetPrefix
        }
      }
    ]
  }
}

//create nic
resource vmNic 'Microsoft.Network/networkInterfaces@2017-06-01' = {
  name: defaultVmNicName
  location: defaultLocation
  properties: {
    ipConfigurations: [
      {
        name: 'ipconfig1'
        properties: {
          subnet: {
            id: '${vnet.id}/subnets/${vnetConfig.subnet.name}'
          }
          privateIPAllocationMethod: privateIPAllocationMethod
        }
      }
    ]
  }
}

//create VM
resource vm 'Microsoft.Compute/virtualMachines@2019-07-01' = {
  name: vmName
  location: defaultLocation
  properties: {
    osProfile: {
      computerName: vmName
      adminUsername: localAdminName
      adminPassword: localAdminPassword
    }
    hardwareProfile: {
      vmSize: vmSku
    }
    storageProfile: {
      imageReference: {
        publisher: 'MicrosoftWindowsDesktop'
        offer: 'Windows-10'
        sku: vmOS
        version: 'latest'
      }
      osDisk: {
        createOption: 'FromImage'
      }
    }
    licenseType: 'Windows_Client'
    networkProfile: {
      networkInterfaces: [
        {
          properties: {
            primary: true
          }
          id: vmNic.id
        }
      ]
    }
  }
}

//add Nvidia drivers using extension in condition is true
resource vmgpu 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = if (installNVidiaGPUDriver) {
  name: '${vm.name}/NvidiaGpuDriverWindows'
  location: defaultLocation
  properties: {
    publisher: 'Microsoft.HpcCompute'
    type: 'NvidiaGpuDriverWindows'
    typeHandlerVersion: '1.3'
    autoUpgradeMinorVersion: true
    settings: {}
  }
}
