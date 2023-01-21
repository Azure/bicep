var vmProperties = {
//@[00:804) ProgramExpression
//@[00:187) ├─DeclaredVariableExpression { Name = vmProperties }
//@[19:187) | └─ObjectExpression
  diagnosticsProfile: {
//@[02:130) |   ├─ObjectPropertyExpression
//@[02:020) |   | ├─StringLiteralExpression { Value = diagnosticsProfile }
//@[22:130) |   | └─ObjectExpression
    bootDiagnostics: {
//@[04:100) |   |   └─ObjectPropertyExpression
//@[04:019) |   |     ├─StringLiteralExpression { Value = bootDiagnostics }
//@[21:100) |   |     └─ObjectExpression
      enabled: 123
//@[06:018) |   |       ├─ObjectPropertyExpression
//@[06:013) |   |       | ├─StringLiteralExpression { Value = enabled }
//@[15:018) |   |       | └─IntegerLiteralExpression { Value = 123 }
      storageUri: true
//@[06:022) |   |       ├─ObjectPropertyExpression
//@[06:016) |   |       | ├─StringLiteralExpression { Value = storageUri }
//@[18:022) |   |       | └─BooleanLiteralExpression { Value = True }
      unknownProp: 'asdf'
//@[06:025) |   |       └─ObjectPropertyExpression
//@[06:017) |   |         ├─StringLiteralExpression { Value = unknownProp }
//@[19:025) |   |         └─StringLiteralExpression { Value = asdf }
    }
  }
  evictionPolicy: 'Deallocate'
//@[02:030) |   └─ObjectPropertyExpression
//@[02:016) |     ├─StringLiteralExpression { Value = evictionPolicy }
//@[18:030) |     └─StringLiteralExpression { Value = Deallocate }
}
resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
//@[00:164) └─DeclaredResourceExpression
//@[61:164)   └─ObjectExpression
  name: 'vm'
  location: 'West US'
//@[02:021)     ├─ObjectPropertyExpression
//@[02:010)     | ├─StringLiteralExpression { Value = location }
//@[12:021)     | └─StringLiteralExpression { Value = West US }
#disable-next-line BCP036 BCP037
  properties: vmProperties
//@[02:026)     └─ObjectPropertyExpression
//@[02:012)       ├─StringLiteralExpression { Value = properties }
//@[14:026)       └─VariableReferenceExpression { Variable = vmProperties }
}
#disable-next-line no-unused-params
param storageAccount1 string = 'testStorageAccount'
//@[00:051) ├─DeclaredParameterExpression { Name = storageAccount1 }
//@[31:051) | └─StringLiteralExpression { Value = testStorageAccount }
#disable-next-line          no-unused-params
param storageAccount2 string = 'testStorageAccount'
//@[00:051) ├─DeclaredParameterExpression { Name = storageAccount2 }
//@[31:051) | └─StringLiteralExpression { Value = testStorageAccount }
#disable-next-line   no-unused-params                /* Test comment 1 */
param storageAccount3 string = 'testStorageAccount'
//@[00:051) ├─DeclaredParameterExpression { Name = storageAccount3 }
//@[31:051) | └─StringLiteralExpression { Value = testStorageAccount }
         #disable-next-line   no-unused-params                // Test comment 2
param storageAccount5 string = 'testStorageAccount'
//@[00:051) ├─DeclaredParameterExpression { Name = storageAccount5 }
//@[31:051) | └─StringLiteralExpression { Value = testStorageAccount }
