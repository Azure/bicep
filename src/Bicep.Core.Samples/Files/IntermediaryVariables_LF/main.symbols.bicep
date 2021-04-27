var boolVal = true
//@[4:11) Variable boolVal. Type: bool. Declaration start char: 0, length: 18

var vmProperties = {
//@[4:16) Variable vmProperties. Type: object. Declaration start char: 0, length: 173
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
//@[9:11) Resource vm. Type: Microsoft.Compute/virtualMachines@2020-12-01. Declaration start char: 0, length: 126
  name: 'vm'
  location: 'West US'
  properties: vmProperties
}

var ipConfigurations = [for i in range(0, 2): {
//@[28:29) Local i. Type: int. Declaration start char: 28, length: 1
//@[4:20) Variable ipConfigurations. Type: object[]. Declaration start char: 0, length: 148
  id: true
  name: 'asdf${i}'
  properties: {
    madeUpProperty: boolVal
    subnet: 'hello'
  }
}]

resource nic 'Microsoft.Network/networkInterfaces@2020-11-01' = {
//@[9:12) Resource nic. Type: Microsoft.Network/networkInterfaces@2020-11-01. Declaration start char: 0, length: 140
  name: 'abc'
  properties: {
    ipConfigurations: ipConfigurations
  }
}

resource nicLoop 'Microsoft.Network/networkInterfaces@2020-11-01' = [for i in range(0, 2): {
//@[73:74) Local i. Type: int. Declaration start char: 73, length: 1
//@[9:16) Resource nicLoop. Type: Microsoft.Network/networkInterfaces@2020-11-01[]. Declaration start char: 0, length: 213
  name: 'abc${i}'
  properties: {
    ipConfigurations: [
      // TODO: fix this
      ipConfigurations[i]
    ]
  }
}]

resource nicLoop2 'Microsoft.Network/networkInterfaces@2020-11-01' = [for ipConfig in ipConfigurations: {
//@[74:82) Local ipConfig. Type: object. Declaration start char: 74, length: 8
//@[9:17) Resource nicLoop2. Type: Microsoft.Network/networkInterfaces@2020-11-01[]. Declaration start char: 0, length: 227
  name: 'abc${ipConfig.name}'
  properties: {
    ipConfigurations: [
      // TODO: fix this
      ipConfig
    ]
  }
}]

