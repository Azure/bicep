// Windows Virtual Machine
resource ${1:windowsVM} 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  properties: {
    hardwareProfile: {
      vmSize: 'Standard_A2_v2'
    }
    osProfile: {
      computerName: ${3:'computerName'}
      adminUsername: ${4:'adminUsername'}
      adminPassword: ${5:'adminPassword'}
    }
    storageProfile: {
      imageReference: {
        publisher: 'MicrosoftWindowsServer'
        offer: 'WindowsServer'
        sku: '2012-R2-Datacenter'
        version: 'latest'
      }
      osDisk: {
        name: ${6:'name'}
        caching: 'ReadWrite'
        createOption: 'FromImage'
      }
    }
    networkProfile: {
      networkInterfaces: [
        {
          id: ${7:'id'}
        }
      ]
    }
    diagnosticsProfile: {
      bootDiagnostics: {
        enabled: true
        storageUri:  ${8:'storageUri'}
      }
    }
  }
}
