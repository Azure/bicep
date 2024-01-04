type foo = resource<'Microsoft.Storage/storageAccounts@2023-01-01'>
//@[00:356) ProgramExpression
//@[00:067) ├─DeclaredTypeExpression { Name = foo }
//@[11:067) | └─ResourceDerivedTypeExpression { Name = Microsoft.Storage/storageAccounts }

param bar resource<'Microsoft.Resources/tags@2022-09-01'> = {
//@[00:160) ├─DeclaredParameterExpression { Name = bar }
//@[10:057) | ├─ResourceDerivedTypeExpression { Name = Microsoft.Resources/tags }
//@[60:160) | └─ObjectExpression
  name: 'default'
//@[02:017) |   ├─ObjectPropertyExpression
//@[02:006) |   | ├─StringLiteralExpression { Value = name }
//@[08:017) |   | └─StringLiteralExpression { Value = default }
  properties: {
//@[02:078) |   └─ObjectPropertyExpression
//@[02:012) |     ├─StringLiteralExpression { Value = properties }
//@[14:078) |     └─ObjectExpression
    tags: {
//@[04:058) |       └─ObjectPropertyExpression
//@[04:008) |         ├─StringLiteralExpression { Value = tags }
//@[10:058) |         └─ObjectExpression
      fizz: 'buzz'
//@[06:018) |           ├─ObjectPropertyExpression
//@[06:010) |           | ├─StringLiteralExpression { Value = fizz }
//@[12:018) |           | └─StringLiteralExpression { Value = buzz }
      snap: 'crackle'
//@[06:021) |           └─ObjectPropertyExpression
//@[06:010) |             ├─StringLiteralExpression { Value = snap }
//@[12:021) |             └─StringLiteralExpression { Value = crackle }
    }
  }
}

output baz resource<'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31'> = {
//@[00:124) └─DeclaredOutputExpression { Name = baz }
//@[11:082)   ├─ResourceDerivedTypeExpression { Name = Microsoft.ManagedIdentity/userAssignedIdentities }
//@[85:124)   └─ObjectExpression
  name: 'myId'
//@[02:014)     ├─ObjectPropertyExpression
//@[02:006)     | ├─StringLiteralExpression { Value = name }
//@[08:014)     | └─StringLiteralExpression { Value = myId }
  location: 'eastus'
//@[02:020)     └─ObjectPropertyExpression
//@[02:010)       ├─StringLiteralExpression { Value = location }
//@[12:020)       └─StringLiteralExpression { Value = eastus }
}

