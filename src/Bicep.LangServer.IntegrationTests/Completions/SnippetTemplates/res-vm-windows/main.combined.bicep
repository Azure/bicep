// $1 = windowsVM
// $2 = 'name'
// $3 = 'computerName'
// $4 = 'adminUsername'
// $5 = 'adminPassword'
// $6 = 'name'
// $7 = 'id'
// $8 = 'storageUri'

resource windowsVM 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    hardwareProfile: {
      vmSize: 'Standard_A2_v2'
    }
    osProfile: {
      computerName: 'computerName'
      adminUsername: 'adminUsername'
//@[21:36) [adminusername-should-not-be-literal (Warning)] When setting an adminUserName property, don't use a literal value. Found literal string value "adminUsername" (CodeDescription: bicep core(https://aka.ms/bicep/linter/adminusername-should-not-be-literal)) |'adminUsername'|
      adminPassword: 'adminPassword'
    }
    storageProfile: {
      imageReference: {
        publisher: 'MicrosoftWindowsServer'
        offer: 'WindowsServer'
        sku: '2012-R2-Datacenter'
        version: 'latest'
      }
      osDisk: {
        name: 'name'
        caching: 'ReadWrite'
        createOption: 'FromImage'
      }
    }
    networkProfile: {
      networkInterfaces: [
        {
          id: 'id'
        }
      ]
    }
    diagnosticsProfile: {
      bootDiagnostics: {
        enabled: true
        storageUri:  'storageUri'
      }
    }
  }
}
// Insert snippet here

