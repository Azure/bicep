param storageCount int = 2
param dataDiskCount int = 2
param vmCount int = 2
param storagePrefix string
param vmPrefix string
param vmSize string

@secure()
param adminPassword string

var items = [
  'a'
  'b'
  'c'
]
var itemTest = [for item in items: item]
var indexTest = [for i in range(0, length(items)): i]

resource storagePrefix_id 'Microsoft.Storage/storageAccounts@2019-04-01' = [for i in range(0, storageCount): {
  name: toLower(concat(i, storagePrefix, uniqueString(resourceGroup().id)))
  location: resourceGroup().location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'Storage'
  properties: {}
}]

resource vmPrefix_resource 'Microsoft.Compute/virtualMachines@2020-06-01' = [for i in range(0, vmCount): {
  name: '${vmPrefix}-${i}'
  location: resourceGroup().location
  properties: {
    hardwareProfile: {
      vmSize: vmSize
    }
    osProfile: {
      computerName: '${vmPrefix}-${(i + 13)}'
      adminUsername: 'vmadmin'
      adminPassword: adminPassword
    }
    storageProfile: {
      imageReference: {
        publisher: 'MicrosoftWindowsServer'
        offer: 'WindowsServer'
        sku: '2016-Datacenter'
        version: 'latest'
      }
      osDisk: {
        createOption: 'FromImage'
      }
      dataDisks: [for j in range(0, dataDiskCount): {
        diskSizeGB: 1023
        lun: (j + 17)
        createOption: 'Empty'
      }]
    }
    networkProfile: {
      networkInterfaces: [
        {
          id: resourceId('Microsoft.Network/networkInterfaces', '${vmPrefix}-${i}')
        }
      ]
    }
  }
}]

resource namedcopy_blah_id 'Microsoft.Storage/storageAccounts@2019-04-01' = [for i in range(0, storageCount): {
  name: toLower('${i}blah${uniqueString(resourceGroup().id)}')
  location: resourceGroup().location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'Storage'
  properties: {}
}]

output myVar array = [for (item, i) in items: {
  name: '>${item}<'
  value: item
  index: i
}]
