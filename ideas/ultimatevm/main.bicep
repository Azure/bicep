input string location

variable virtualNetworkName 'vnet'
variable addressPrefix '10.0.0.0/16'
variable subnetName 'Subnet'
variable subnetPrefix '10.0.0.0/24'

resource azrm 'network/virtualNetworks@2018-07-01' vnet {
  name: virtualNetworkName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        addressPrefix
      ]
    }
    subnets: [
      {
        name: subnetName
        properties: {
          addressPrefix: subnetPrefix
        }
      }
    ]
  }
}

for i in range(0, numberOfVms) {
  resource mod 'module@virtualMachine' vmMod {
    location: location
    adminPasswordOrKey: adminPasswordOrKey
    adminUsername: adminUsername
    vmName: 'vm-${i}'
    zones: '${1 + (i % 3)}'
    subnetId: vnet.properties.subnets[0].id
  }

  resource mod 'module@customScript' csMod {
    location: location
    vmName: 'vm-${i}'
    isWindowsOS: false
    fileUris: uri(artifactsLocation, 'scripts/init.sh${artifactsLocationSasToken}')
    commandToExecute: 'bash init.sh'
  }
}