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
//@[04:12) [no-unused-vars (Warning)] Variable "itemTest" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |itemTest|
var indexTest = [for i in range(0, length(items)): i]
//@[04:13) [no-unused-vars (Warning)] Variable "indexTest" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |indexTest|

resource storagePrefix_id 'Microsoft.Storage/storageAccounts@2019-04-01' = [for i in range(0, storageCount): {
  name: toLower(concat(i, storagePrefix, uniqueString(resourceGroup().id)))
//@[16:74) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(i, storagePrefix, uniqueString(resourceGroup().id))|
  location: resourceGroup().location
//@[12:36) [no-loc-expr-outside-params (Warning)] Use a parameter here instead of 'resourceGroup().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-loc-expr-outside-params)) |resourceGroup().location|
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'Storage'
  properties: {
  }
}]

resource vmPrefix_resource 'Microsoft.Compute/virtualMachines@2020-06-01' = [for i in range(0, vmCount): {
  name: '${vmPrefix}-${i}'
  location: resourceGroup().location
//@[12:36) [no-loc-expr-outside-params (Warning)] Use a parameter here instead of 'resourceGroup().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-loc-expr-outside-params)) |resourceGroup().location|
  properties: {
    hardwareProfile: {
      vmSize: vmSize
    }
    osProfile: {
      computerName: '${vmPrefix}-${(i + 13)}'
      adminUsername: 'vmadmin'
//@[21:30) [adminusername-should-not-be-literal (Warning)] Property 'adminUserName' should not use a literal value. Use a param instead. Found literal string value "vmadmin" (CodeDescription: bicep core(https://aka.ms/bicep/linter/adminusername-should-not-be-literal)) |'vmadmin'|
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
//@[12:36) [no-loc-expr-outside-params (Warning)] Use a parameter here instead of 'resourceGroup().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-loc-expr-outside-params)) |resourceGroup().location|
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'Storage'
  properties: {
  }
}]

output myVar array = [for (item, i) in items: {
  name: '>${item}<'
  value: item
  index: i
}]
