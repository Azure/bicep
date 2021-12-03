// Windows Virtual Machine
resource /*${1:windowsVM}*/windowsVM 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  properties: {
    hardwareProfile: {
      vmSize: 'Standard_A2_v2'
    }
    osProfile: {
      computerName: /*${3:'computerName'}*/'computerName'
      adminUsername: /*${4:'adminUsername'}*/'adminUsername'
      adminPassword: /*${5:'adminPassword'}*/'adminPassword'
    }
    storageProfile: {
      imageReference: {
        publisher: 'MicrosoftWindowsServer'
        offer: 'WindowsServer'
        sku: '2012-R2-Datacenter'
        version: 'latest'
      }
      osDisk: {
        name: /*${6:'name'}*/'name'
        caching: 'ReadWrite'
        createOption: 'FromImage'
      }
    }
    networkProfile: {
      networkInterfaces: [
        {
          id: /*${7:'id'}*/'id'
        }
      ]
    }
    diagnosticsProfile: {
      bootDiagnostics: {
        enabled: true
        storageUri:  /*${8:'storageUri'}*/'storageUri'
      }
    }
  }
}
