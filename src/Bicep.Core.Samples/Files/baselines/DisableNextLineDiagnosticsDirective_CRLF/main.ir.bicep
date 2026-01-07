var vmProperties = {
//@[00:1294) ProgramExpression
//@[00:0187) ├─DeclaredVariableExpression { Name = vmProperties }
//@[19:0187) | └─ObjectExpression
  diagnosticsProfile: {
//@[02:0130) |   ├─ObjectPropertyExpression
//@[02:0020) |   | ├─StringLiteralExpression { Value = diagnosticsProfile }
//@[22:0130) |   | └─ObjectExpression
    bootDiagnostics: {
//@[04:0100) |   |   └─ObjectPropertyExpression
//@[04:0019) |   |     ├─StringLiteralExpression { Value = bootDiagnostics }
//@[21:0100) |   |     └─ObjectExpression
      enabled: 123
//@[06:0018) |   |       ├─ObjectPropertyExpression
//@[06:0013) |   |       | ├─StringLiteralExpression { Value = enabled }
//@[15:0018) |   |       | └─IntegerLiteralExpression { Value = 123 }
      storageUri: true
//@[06:0022) |   |       ├─ObjectPropertyExpression
//@[06:0016) |   |       | ├─StringLiteralExpression { Value = storageUri }
//@[18:0022) |   |       | └─BooleanLiteralExpression { Value = True }
      unknownProp: 'asdf'
//@[06:0025) |   |       └─ObjectPropertyExpression
//@[06:0017) |   |         ├─StringLiteralExpression { Value = unknownProp }
//@[19:0025) |   |         └─StringLiteralExpression { Value = asdf }
    }
  }
  evictionPolicy: 'Deallocate'
//@[02:0030) |   └─ObjectPropertyExpression
//@[02:0016) |     ├─StringLiteralExpression { Value = evictionPolicy }
//@[18:0030) |     └─StringLiteralExpression { Value = Deallocate }
}
resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
//@[00:0164) └─DeclaredResourceExpression
//@[61:0164)   └─ObjectExpression
  name: 'vm'
  location: 'West US'
//@[02:0021)     ├─ObjectPropertyExpression
//@[02:0010)     | ├─StringLiteralExpression { Value = location }
//@[12:0021)     | └─StringLiteralExpression { Value = West US }
#disable-next-line BCP036 BCP037
  properties: vmProperties
//@[02:0026)     └─ObjectPropertyExpression
//@[02:0012)       ├─StringLiteralExpression { Value = properties }
//@[14:0026)       └─VariableReferenceExpression { Variable = vmProperties }
}
#disable-next-line no-unused-params
param storageAccount1 string = 'testStorageAccount'
//@[00:0051) ├─DeclaredParameterExpression { Name = storageAccount1 }
//@[22:0028) | ├─AmbientTypeReferenceExpression { Name = string }
//@[31:0051) | └─StringLiteralExpression { Value = testStorageAccount }
#disable-next-line          no-unused-params
param storageAccount2 string = 'testStorageAccount'
//@[00:0051) ├─DeclaredParameterExpression { Name = storageAccount2 }
//@[22:0028) | ├─AmbientTypeReferenceExpression { Name = string }
//@[31:0051) | └─StringLiteralExpression { Value = testStorageAccount }
#disable-next-line   no-unused-params                /* Test comment 1 */
param storageAccount3 string = 'testStorageAccount'
//@[00:0051) ├─DeclaredParameterExpression { Name = storageAccount3 }
//@[22:0028) | ├─AmbientTypeReferenceExpression { Name = string }
//@[31:0051) | └─StringLiteralExpression { Value = testStorageAccount }
         #disable-next-line   no-unused-params                // Test comment 2
param storageAccount5 string = 'testStorageAccount'
//@[00:0051) ├─DeclaredParameterExpression { Name = storageAccount5 }
//@[22:0028) | ├─AmbientTypeReferenceExpression { Name = string }
//@[31:0051) | └─StringLiteralExpression { Value = testStorageAccount }

#disable-diagnostics                 no-unused-params                      no-unused-vars
param storageAccount4 string = 'testStorageAccount'
//@[00:0051) ├─DeclaredParameterExpression { Name = storageAccount4 }
//@[22:0028) | ├─AmbientTypeReferenceExpression { Name = string }
//@[31:0051) | └─StringLiteralExpression { Value = testStorageAccount }
var unusedVar1 = 'This is an unused variable'
//@[00:0045) ├─DeclaredVariableExpression { Name = unusedVar1 }
//@[17:0045) | └─StringLiteralExpression { Value = This is an unused variable }
var unusedVar2 = 'This is another unused variable'
//@[00:0050) ├─DeclaredVariableExpression { Name = unusedVar2 }
//@[17:0050) | └─StringLiteralExpression { Value = This is another unused variable }
#restore-diagnostics   no-unused-vars
param storageAccount6 string = 'testStorageAccount'
//@[00:0051) ├─DeclaredParameterExpression { Name = storageAccount6 }
//@[22:0028) | ├─AmbientTypeReferenceExpression { Name = string }
//@[31:0051) | └─StringLiteralExpression { Value = testStorageAccount }
var unusedVar3 = 'This is yet another unused variable'
//@[00:0054) ├─DeclaredVariableExpression { Name = unusedVar3 }
//@[17:0054) | └─StringLiteralExpression { Value = This is yet another unused variable }
#restore-diagnostics    no-unused-params
param storageAccount7 string = 'testStorageAccount'
//@[00:0051) ├─DeclaredParameterExpression { Name = storageAccount7 }
//@[22:0028) | ├─AmbientTypeReferenceExpression { Name = string }
//@[31:0051) | └─StringLiteralExpression { Value = testStorageAccount }

