param location string = resourceGroup().location
param csadminSshKey string
param customData string = ''
param subnetId string
param userAssignedIdentity string

resource pip 'Microsoft.Network/publicIpAddresses@2020-05-01' = {
  name: 'cycleserver-pip'
  location: location
  properties: {
    publicIPAllocationMethod: 'Dynamic'
  }
}

resource nic 'Microsoft.Network/networkInterfaces@2020-05-01' = {
  name: 'cycleserver-nic'
  location: location
  properties: {
    ipConfigurations: [
      {
        name: 'ipconfig'
        properties: {
          privateIPAllocationMethod: 'Dynamic'
          subnet: {
            id: subnetId
          }
          publicIPAddress: {
            id: pip.id
          }
        }
      }
    ]
  }
}

resource vm 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  name: 'cycleserver'
  location: location
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${userAssignedIdentity}': {}
    }
  }
  properties: {
    osProfile: {
      computerName: 'CycleServer'
      adminUsername: 'csadmin'
      customData: base64(customData)
      linuxConfiguration: {
        disablePasswordAuthentication: true
        ssh: {
          publicKeys: [
            {
              path: '/home/csadmin/.ssh/authorized_keys'
              keyData: csadminSshKey
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
        sku: '8_2'
        version: 'latest'
      }
      osDisk: {
        name: 'cycleserver-os'
        createOption: 'FromImage'
        managedDisk: {
          storageAccountType: 'StandardSSD_LRS'
        }
      }
    }
    networkProfile: {
      networkInterfaces: [
        {
          id: nic.id
        }
      ]
    }
  }
}
