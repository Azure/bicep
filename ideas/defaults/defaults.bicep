export resource azrm 'compute/virtualMachines@2019-07-01' basicVm {
  name: input.name
  location: input.location
  properties: {
    osProfile: {
      computerName: input.name
      adminUsername: input.adminUsername
      adminPassword: input.adminPassword
      windowsConfiguration: {
        provisionVMAgent: true
      }
    }
    hardwareProfile: {
      vmSize: 'Standard_A1_v2'
    }
    storageProfile: {
      imageReference: {
        publisher: 'MicrosoftWindowsServer',
        offer: 'WindowsServer'
        sku: '2016-Datacenter'
        version: 'latest'
      },
      osDisk: {
        createOption: 'FromImage'
      }
      dataDisks: []
    }
    networkProfile: {
      networkInterfaces: [
        properties: {
          primary: true
        }
        id: input.networkInterface.id
      ]
    },
    diagnosticsProfile: {
      bootDiagnostics: {
        enabled: true
        id: input.storageAccount.id
      }
    }
  }
}

export resource azrm 'network/networkInterfaces@2019-11-01' basicNic {
  name: input.name
  location: input.location
  properties: {
    ipConfigurations: [
      {
        name: 'ipConfig'
        properties: {
          subnet: {
            id: input.subnet.id
          }
          privateIPAllocationMethod: 'Dynamic'
        }
      }
    ]
  }
}

export resource azrm 'storage/storageAccounts@2015-06-15' basicStorage {
  name: input.name
  location: input.location
  properties: {
    accountType: 'Standard_LRS'
  }
}