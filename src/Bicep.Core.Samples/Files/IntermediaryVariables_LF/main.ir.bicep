var boolVal = true
//@[000:1058) ProgramExpression
//@[000:0018) ├─DeclaredVariableExpression { Name = boolVal }
//@[014:0018) | └─BooleanLiteralExpression { Value = True }

var vmProperties = {
//@[000:0173) ├─DeclaredVariableExpression { Name = vmProperties }
//@[019:0173) | └─ObjectExpression
  diagnosticsProfile: {
//@[002:0124) |   ├─ObjectPropertyExpression
//@[002:0020) |   | ├─StringLiteralExpression { Value = diagnosticsProfile }
//@[022:0124) |   | └─ObjectExpression
    bootDiagnostics: {
//@[004:0096) |   |   └─ObjectPropertyExpression
//@[004:0019) |   |     ├─StringLiteralExpression { Value = bootDiagnostics }
//@[021:0096) |   |     └─ObjectExpression
      enabled: 123
//@[006:0018) |   |       ├─ObjectPropertyExpression
//@[006:0013) |   |       | ├─StringLiteralExpression { Value = enabled }
//@[015:0018) |   |       | └─IntegerLiteralExpression { Value = 123 }
      storageUri: true
//@[006:0022) |   |       ├─ObjectPropertyExpression
//@[006:0016) |   |       | ├─StringLiteralExpression { Value = storageUri }
//@[018:0022) |   |       | └─BooleanLiteralExpression { Value = True }
      unknownProp: 'asdf'
//@[006:0025) |   |       └─ObjectPropertyExpression
//@[006:0017) |   |         ├─StringLiteralExpression { Value = unknownProp }
//@[019:0025) |   |         └─StringLiteralExpression { Value = asdf }
    }
  }
  evictionPolicy: boolVal
//@[002:0025) |   └─ObjectPropertyExpression
//@[002:0016) |     ├─StringLiteralExpression { Value = evictionPolicy }
//@[018:0025) |     └─VariableReferenceExpression { Variable = boolVal }
}

resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
//@[000:0126) ├─DeclaredResourceExpression
//@[061:0126) | └─ObjectExpression
  name: 'vm'
  location: 'West US'
//@[002:0021) |   ├─ObjectPropertyExpression
//@[002:0010) |   | ├─StringLiteralExpression { Value = location }
//@[012:0021) |   | └─StringLiteralExpression { Value = West US }
  properties: vmProperties
//@[002:0026) |   └─ObjectPropertyExpression
//@[002:0012) |     ├─StringLiteralExpression { Value = properties }
//@[014:0026) |     └─VariableReferenceExpression { Variable = vmProperties }
}

var ipConfigurations = [for i in range(0, 2): {
//@[000:0148) ├─DeclaredVariableExpression { Name = ipConfigurations }
//@[023:0148) | └─ForLoopExpression
//@[033:0044) |   ├─FunctionCallExpression { Name = range }
//@[039:0040) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[042:0043) |   | └─IntegerLiteralExpression { Value = 2 }
//@[046:0147) |   └─ObjectExpression
//@[033:0044) |     |     └─FunctionCallExpression { Name = range }
//@[039:0040) |     |       ├─IntegerLiteralExpression { Value = 0 }
//@[042:0043) |     |       └─IntegerLiteralExpression { Value = 2 }
  id: true
//@[002:0010) |     ├─ObjectPropertyExpression
//@[002:0004) |     | ├─StringLiteralExpression { Value = id }
//@[006:0010) |     | └─BooleanLiteralExpression { Value = True }
  name: 'asdf${i}'
//@[002:0018) |     ├─ObjectPropertyExpression
//@[002:0006) |     | ├─StringLiteralExpression { Value = name }
//@[008:0018) |     | └─InterpolatedStringExpression
//@[015:0016) |     |   └─ArrayAccessExpression
//@[015:0016) |     |     ├─CopyIndexExpression
  properties: {
//@[002:0067) |     └─ObjectPropertyExpression
//@[002:0012) |       ├─StringLiteralExpression { Value = properties }
//@[014:0067) |       └─ObjectExpression
    madeUpProperty: boolVal
//@[004:0027) |         ├─ObjectPropertyExpression
//@[004:0018) |         | ├─StringLiteralExpression { Value = madeUpProperty }
//@[020:0027) |         | └─VariableReferenceExpression { Variable = boolVal }
    subnet: 'hello'
//@[004:0019) |         └─ObjectPropertyExpression
//@[004:0010) |           ├─StringLiteralExpression { Value = subnet }
//@[012:0019) |           └─StringLiteralExpression { Value = hello }
  }
}]

resource nic 'Microsoft.Network/networkInterfaces@2020-11-01' = {
//@[000:0140) ├─DeclaredResourceExpression
//@[064:0140) | └─ObjectExpression
  name: 'abc'
  properties: {
//@[002:0058) |   └─ObjectPropertyExpression
//@[002:0012) |     ├─StringLiteralExpression { Value = properties }
//@[014:0058) |     └─ObjectExpression
    ipConfigurations: ipConfigurations
//@[004:0038) |       └─ObjectPropertyExpression
//@[004:0020) |         ├─StringLiteralExpression { Value = ipConfigurations }
//@[022:0038) |         └─VariableReferenceExpression { Variable = ipConfigurations }
  }
}

resource nicLoop 'Microsoft.Network/networkInterfaces@2020-11-01' = [for i in range(0, 2): {
//@[000:0213) ├─DeclaredResourceExpression
//@[068:0213) | └─ForLoopExpression
//@[078:0089) |   ├─FunctionCallExpression { Name = range }
//@[084:0085) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[087:0088) |   | └─IntegerLiteralExpression { Value = 2 }
//@[091:0212) |   └─ObjectExpression
//@[078:0089) |               | └─FunctionCallExpression { Name = range }
//@[084:0085) |               |   ├─IntegerLiteralExpression { Value = 0 }
//@[087:0088) |               |   └─IntegerLiteralExpression { Value = 2 }
  name: 'abc${i}'
  properties: {
//@[002:0099) |     └─ObjectPropertyExpression
//@[002:0012) |       ├─StringLiteralExpression { Value = properties }
//@[014:0099) |       └─ObjectExpression
    ipConfigurations: [
//@[004:0079) |         └─ObjectPropertyExpression
//@[004:0020) |           ├─StringLiteralExpression { Value = ipConfigurations }
//@[022:0079) |           └─ArrayExpression
      // TODO: fix this
      ipConfigurations[i]
//@[006:0025) |             └─ArrayAccessExpression
//@[023:0024) |               ├─ArrayAccessExpression
//@[023:0024) |               | ├─CopyIndexExpression
//@[006:0022) |               └─VariableReferenceExpression { Variable = ipConfigurations }
    ]
  }
}]

resource nicLoop2 'Microsoft.Network/networkInterfaces@2020-11-01' = [for ipConfig in ipConfigurations: {
//@[000:0227) └─DeclaredResourceExpression
//@[069:0227)   └─ForLoopExpression
//@[086:0102)     ├─VariableReferenceExpression { Variable = ipConfigurations }
//@[104:0226)     └─ObjectExpression
//@[086:0102)                 └─VariableReferenceExpression { Variable = ipConfigurations }
  name: 'abc${ipConfig.name}'
  properties: {
//@[002:0088)       └─ObjectPropertyExpression
//@[002:0012)         ├─StringLiteralExpression { Value = properties }
//@[014:0088)         └─ObjectExpression
    ipConfigurations: [
//@[004:0068)           └─ObjectPropertyExpression
//@[004:0020)             ├─StringLiteralExpression { Value = ipConfigurations }
//@[022:0068)             └─ArrayExpression
      // TODO: fix this
      ipConfig
//@[006:0014)               └─ArrayAccessExpression
//@[006:0014)                 ├─CopyIndexExpression
    ]
  }
}]

