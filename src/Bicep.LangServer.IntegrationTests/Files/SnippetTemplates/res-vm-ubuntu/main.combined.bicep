// $1 = ubuntuVM
// $2 = 'name'
// $3 = location
// $4 = 'computerName'
// $5 = adminUsername
// $6 = adminPassword
// $7 = 'name'
// $8 = 'id'
// $9 = 'storageUri'

param location string
@secure()
param adminPassword string
param adminUsername string

resource ubuntuVM 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'name'
  location: location
  properties: {
    hardwareProfile: {
      vmSize: 'Standard_A2_v2'
    }
    osProfile: {
      computerName: 'computerName'
      adminUsername: adminUsername
      adminPassword: adminPassword
//@[21:34) [BCP417 (Info)] The supplied value has been marked as secure but is being assigned to a target that is not expecting sensitive data. (bicep https://aka.ms/bicep/core-diagnostics#BCP417) |adminPassword|
    }
    storageProfile: {
      imageReference: {
        publisher: 'Canonical'
        offer: 'UbuntuServer'
        sku: '16.04-LTS'
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
        storageUri: 'storageUri'
      }
    }
  }
}


