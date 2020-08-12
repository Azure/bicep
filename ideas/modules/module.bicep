module basicStorageAccount {
  input accountName string
  input location string

  resource azrm 'storage/storageAccounts@2015-06-15' default {
    name: input.accountName
    location: input.location
    properties: {
      accountType: 'Standard_LRS'
    }
  }
}

export module vmWithDiags {
  input accountName string
  input location string

  resource module 'basicStorageAccount' diagsAccount {
    input: {
      accountName: input.namePrefix + '-sa'
      location: input.location
    }
    /* optional other syntax:
    input.accountName: input.namePrefix + '-sa'
    input.location: input.location
    */
  }

  resource azrm 'network/networkInterfaces@2019-11-01' mainNic {
    name: input.namePrefix + '-nic'
    properties: {
      ipConfigurations: [
        {
          name: 'ipConfig'
          properties: {
            subnet: {
              id: input.subnetResourceId
            }
            privateIPAllocationMethod: 'Dynamic'
          }
        }
      ]
    }
  }

  resource azrm 'compute/virtualMachines@2019-07-01' mainVm {
    name: input.namePrefix + '-vm'
    location: input.location
    properties: {
      osProfile: {
        computerName: input.namePrefix
        adminUsername: input.namePrefix + 'admin'
        adminPassword: 'myS3cretP@ssw0rd'
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
          id: mainNic.id
        ]
      },
      diagnosticsProfile: {
        bootDiagnostics: {
          enabled: true
          // can we fully infer dependency order using references?
          id: diagsAccount.id
        }
      }
    }
  }

  output blobUrl diagsAccount.primaryEndpoints.blob
}