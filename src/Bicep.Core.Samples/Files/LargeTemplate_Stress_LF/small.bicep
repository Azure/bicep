// no-unused-params
param param11 string
param param22 string
param param33 string
param param43 string

// no-unused-vars and no-hardcoded-env-urls
var location1 = 'http://MANAGEMENT.core.windows.net'
var location2 = 'http://MANAGEMENT.core.windows.net'
var location3 = 'http://MANAGEMENT.core.windows.net'
var location4 = 'http://MANAGEMENT.core.windows.net'

// adminusername-should-not-be-literal
resource vm1 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'name1'
  location: resourceGroup().location
  properties: {
    osProfile: {
      adminUsername: 'adminUsername'
    }
  }
}
resource vm2 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'name2'
  location: resourceGroup().location
  properties: {
    osProfile: {
      adminUsername: 'adminUsername'
    }
  }
}
resource vm3 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'name3'
  location: resourceGroup().location
  properties: {
    osProfile: {
      adminUsername: 'adminUsername'
    }
  }
}
resource vm4 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'name4'
  location: resourceGroup().location
  properties: {
    osProfile: {
      adminUsername: 'adminUsername'
    }
  }
}

// no-unnecessary-dependson
resource appServicePlan 'Microsoft.Web/serverfarms@2020-12-01' = {
  name: 'appServicePlan'
  location: resourceGroup().location
  sku: {
    name: 'F1'
    capacity: 1
  }
}
resource webApplication1 'Microsoft.Web/sites@2018-11-01' = {
  name: 'name1'
  location: resourceGroup().location
  properties: {
    serverFarmId: 'appServicePlanId'
    dependsOn: [
      // This should be picked up as a dependency of appServicePlan and not ignored, even though the property name is
      // dependsOn, but it's not a top-level property
      appServicePlan.id
    ]
  }
  dependsOn: [
    appServicePlan // Should fail because we already have reference to appServicePlan.id in non-top-level property dependsOn
  ]
}
resource webApplication11 'Microsoft.Web/sites@2018-11-01' = {
  name: 'name2'
  location: resourceGroup().location
  properties: {
    serverFarmId: 'appServicePlanId'
    dependsOn: [
      // This should be picked up as a dependency of appServicePlan and not ignored, even though the property name is
      // dependsOn, but it's not a top-level property
      appServicePlan.id
    ]
  }
  dependsOn: [
    appServicePlan // Should fail because we already have reference to appServicePlan.id in non-top-level property dependsOn
  ]
}
resource webApplication22 'Microsoft.Web/sites@2018-11-01' = {
  name: 'name3'
  location: resourceGroup().location
  properties: {
    serverFarmId: 'appServicePlanId'
    dependsOn: [
      // This should be picked up as a dependency of appServicePlan and not ignored, even though the property name is
      // dependsOn, but it's not a top-level property
      appServicePlan.id
    ]
  }
  dependsOn: [
    appServicePlan // Should fail because we already have reference to appServicePlan.id in non-top-level property dependsOn
  ]
}
resource webApplication3 'Microsoft.Web/sites@2018-11-01' = {
  name: 'name4'
  location: resourceGroup().location
  properties: {
    serverFarmId: 'appServicePlanId'
    dependsOn: [
      // This should be picked up as a dependency of appServicePlan and not ignored, even though the property name is
      // dependsOn, but it's not a top-level property
      appServicePlan.id
    ]
  }
  dependsOn: [
    appServicePlan // Should fail because we already have reference to appServicePlan.id in non-top-level property dependsOn
  ]
}

// outputs-should-not-contain-secrets
@secure()
param secureParam object = {
  value: 'hello'
}
output badResult1 string = 'this is the value ${secureParam.value}'
output badResult2 string = 'this is the value ${secureParam.value}'
output badResult3 string = 'this is the value ${secureParam.value}'
output badResult4 string = 'this is the value ${secureParam.value}'

// prefer-interpolation
param suffix string = '001'
param vnetName1 string = concat('vnet-', suffix)
param vnetName2 string = concat('vnet-', suffix)
param vnetName3 string = concat('vnet-', suffix)
param vnetName4 string = concat('vnet-', suffix)

// secure-parameter-default
@secure()
param param1 string = 'val7'
@secure()
param param2 string = 'val7'
@secure()
param param3 string = 'val7'
@secure()
param param4 string = 'val7'

// simplify-interpolation
param p1 string
var v1 = '${p1}'
var v2 = '${p1}'
var v3 = '${p1}'
var v4 = '${p1}'

// use-protectedsettings-for-commandtoexecute-secrets
param vmName string
param location string
param fileUris string

@secure()
param arguments string = ''

var UriFileNamePieces = split(fileUris, '/')
var firstFileNameString = UriFileNamePieces[(length(UriFileNamePieces) - 1)]
var firstFileNameBreakString = split(firstFileNameString, '?')
var firstFileName = firstFileNameBreakString[0]

resource customScriptExtension1 'Microsoft.Compute/virtualMachines/extensions@2021-04-01' = {
  name: '${vmName}/CustomScriptExtension'
  location: location
  properties: {
    publisher: 'Microsoft.Compute'
    type: 'CustomScriptExtension'
    autoUpgradeMinorVersion: true
    settings: {
      fileUris: split(fileUris, ' ')
      commandToExecute: 'powershell -ExecutionPolicy Unrestricted -File ${firstFileName} ${arguments}' // MODIFIED
    }
  }
}

resource customScriptExtension2 'Microsoft.Compute/virtualMachines/extensions@2021-04-01' = {
  name: '${vmName}/CustomScriptExtension'
  location: location
  properties: {
    publisher: 'Microsoft.Compute'
    type: 'CustomScriptExtension'
    autoUpgradeMinorVersion: true
    settings: {
      fileUris: split(fileUris, ' ')
      commandToExecute: 'powershell -ExecutionPolicy Unrestricted -File ${firstFileName} ${arguments}' // MODIFIED
    }
  }
}

resource customScriptExtension3 'Microsoft.Compute/virtualMachines/extensions@2021-04-01' = {
  name: '${vmName}/CustomScriptExtension'
  location: location
  properties: {
    publisher: 'Microsoft.Compute'
    type: 'CustomScriptExtension'
    autoUpgradeMinorVersion: true
    settings: {
      fileUris: split(fileUris, ' ')
      commandToExecute: 'powershell -ExecutionPolicy Unrestricted -File ${firstFileName} ${arguments}' // MODIFIED
    }
  }
}

resource customScriptExtension4 'Microsoft.Compute/virtualMachines/extensions@2021-04-01' = {
  name: '${vmName}/CustomScriptExtension'
  location: location
  properties: {
    publisher: 'Microsoft.Compute'
    type: 'CustomScriptExtension'
    autoUpgradeMinorVersion: true
    settings: {
      fileUris: split(fileUris, ' ')
      commandToExecute: 'powershell -ExecutionPolicy Unrestricted -File ${firstFileName} ${arguments}' // MODIFIED
    }
  }
}

// use-stable-vm-image
resource test1 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  name: 'virtualMachineName1'
  location: resourceGroup().location
  properties: {
    storageProfile: {
      imageReference: {
        offer: 'WindowsServer'
        sku: '2019-Datacenter'
        version: 'preview'
      }
    }
  }
}
resource test2 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  name: 'virtualMachineName2'
  location: resourceGroup().location
  properties: {
    storageProfile: {
      imageReference: {
        offer: 'WindowsServer'
        sku: '2019-Datacenter'
        version: 'preview'
      }
    }
  }
}
resource test3 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  name: 'virtualMachineName3'
  location: resourceGroup().location
  properties: {
    storageProfile: {
      imageReference: {
        offer: 'WindowsServer'
        sku: '2019-Datacenter'
        version: 'preview'
      }
    }
  }
}
resource test4 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  name: 'virtualMachineName4'
  location: resourceGroup().location
  properties: {
    storageProfile: {
      imageReference: {
        offer: 'WindowsServer'
        sku: '2019-Datacenter'
        version: 'preview'
      }
    }
  }
}
  location: resourceGroup().location
  properties: {
    storageProfile: {
      imageReference: {
        offer: 'WindowsServer'
        sku: '2019-Datacenter'
        version: 'preview'
      }
    }
  }
}
