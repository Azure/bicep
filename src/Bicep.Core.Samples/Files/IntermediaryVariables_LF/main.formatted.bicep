var boolVal = true

var vmProperties = {
  diagnosticsProfile: {
    bootDiagnostics: {
      enabled: 123
      storageUri: true
      unknownProp: 'asdf'
    }
  }
  evictionPolicy: boolVal
}

resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'vm'
  location: 'West US'
  properties: vmProperties
}

var ipConfigurations = [for i in range(0, 2): {
  id: true
  name: 'asdf${i}'
  properties: {
    madeUpProperty: boolVal
    subnet: 'hello'
  }
}]

resource nic 'Microsoft.Network/networkInterfaces@2020-11-01' = {
  name: 'abc'
  properties: {
    ipConfigurations: ipConfigurations
  }
}

resource nicLoop 'Microsoft.Network/networkInterfaces@2020-11-01' = [for i in range(0, 2): {
  name: 'abc${i}'
  properties: {
    ipConfigurations: [
      // TODO: fix this
      ipConfigurations[i]
    ]
  }
}]

resource nicLoop2 'Microsoft.Network/networkInterfaces@2020-11-01' = [for ipConfig in ipConfigurations: {
  name: 'abc${ipConfig.name}'
  properties: {
    ipConfigurations: [
      // TODO: fix this
      ipConfig
    ]
  }
}]
