// Ubuntu Virtual Machine
resource /*${1:ubuntuVM}*/ubuntuVM 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: {
    hardwareProfile: {
      vmSize: 'Standard_A2_v2'
    }
    osProfile: {
      computerName: /*${4:'computerName'}*/'computerName'
      adminUsername: /*${5:'adminUsername'}*/'adminUsername'
      adminPassword: /*${6:'adminPassword'}*/'adminPassword'
    }
    storageProfile: {
      imageReference: {
        publisher: 'Canonical'
        offer: 'UbuntuServer'
        sku: '16.04-LTS'
        version: 'latest'
      }
      osDisk: {
        name: /*${7:'name'}*/'name'
        caching: 'ReadWrite'
        createOption: 'FromImage'
      }
    }
    networkProfile: {
      networkInterfaces: [
        {
          id: /*${8:'id'}*/'id'
        }
      ]
    }
    diagnosticsProfile: {
      bootDiagnostics: {
        enabled: true
        storageUri: /*${9:'storageUri'}*/'storageUri'
      }
    }
  }
}
