// var used to hold sample Cloud-Init config to customise the Linux VM once provisioned, adding AZ CLI in this instance
var customData = '#cloud-config\n\nyum_repos:\n  azure-cli:\n    baseurl: https://packages.microsoft.com/yumrepos/azure-cli\n    enabled: true\n    gpgcheck: true\n    gpgkey: https://packages.microsoft.com/keys/microsoft.asc\n    name: Azure CLI\n\npackages:\n- azure-cli'

resource vm 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  name: 'myvm'
  location: 'eastus'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    osProfile: {
      computerName: 'myvm'
      adminUsername: 'adminuser'
      customData: base64(customData)
      linuxConfiguration: {
        disablePasswordAuthentication: true
        ssh: {
          publicKeys: [
            {
              path: '/home/adminuser/.ssh/authorized_keys'
              keyData: 'ssh public key data'
            }
          ]
        }
      }
    }
    hardwareProfile: {
      vmSize: 'Standard_D8s_v3'
    }
    storageProfile: {
      imageReference: {
        publisher: 'OpenLogic'
        offer: 'CentOS'
        sku: '7.7'
        version: 'latest'
      }
      osDisk: {
        name:  'myvm-os'
        createOption: 'FromImage'
        managedDisk: {
          storageAccountType: 'StandardSSD_LRS'
        }
      }
    }
    networkProfile: {
      networkInterfaces: [
        {
          id: 'nic Id'
        }
      ]
    }
  }
}
  