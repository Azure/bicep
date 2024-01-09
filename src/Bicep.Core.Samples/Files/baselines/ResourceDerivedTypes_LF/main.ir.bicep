type foo = resource<'Microsoft.Storage/storageAccounts@2023-01-01'>
//@[00:974) ProgramExpression
//@[00:067) ├─DeclaredTypeExpression { Name = foo }
//@[11:067) | └─ResourceDerivedTypeExpression { Name = Microsoft.Storage/storageAccounts }

type test = {
//@[00:239) ├─DeclaredTypeExpression { Name = test }
//@[12:239) | └─ObjectTypeExpression { Name = { resA: Microsoft.Storage/storageAccounts, resB: Microsoft.Storage/storageAccounts, resC: array, resD: Microsoft.Storage/storageAccounts } }
  resA: resource<'Microsoft.Storage/storageAccounts@2023-01-01'>
//@[02:064) |   ├─ObjectTypePropertyExpression
//@[08:064) |   | └─ResourceDerivedTypeExpression { Name = Microsoft.Storage/storageAccounts }
  resB: sys.resource<'Microsoft.Storage/storageAccounts@2022-09-01'>
//@[02:068) |   ├─ObjectTypePropertyExpression
//@[08:068) |   | └─ResourceDerivedTypeExpression { Name = Microsoft.Storage/storageAccounts }
  resC: sys.array
//@[02:017) |   ├─ObjectTypePropertyExpression
//@[08:017) |   | └─FullyQualifiedAmbientTypeReferenceExpression { Name = sys.array }
  resD: sys.resource<'az:Microsoft.Storage/storageAccounts@2022-09-01'>
//@[02:071) |   └─ObjectTypePropertyExpression
//@[08:071) |     └─ResourceDerivedTypeExpression { Name = Microsoft.Storage/storageAccounts }
}

type strangeFormattings = {
//@[00:258) ├─DeclaredTypeExpression { Name = strangeFormattings }
//@[26:258) | └─ObjectTypeExpression { Name = { test: Astronomer.Astro/organizations, test2: Microsoft.Storage/storageAccounts, test3: Microsoft.Storage/storageAccounts } }
  test: resource<
//@[02:075) |   ├─ObjectTypePropertyExpression
//@[08:075) |   | └─ResourceDerivedTypeExpression { Name = Astronomer.Astro/organizations }

  'Astronomer.Astro/organizations@2023-08-01-preview'

>
  test2: resource    <'Microsoft.Storage/storageAccounts@2023-01-01'>
//@[02:069) |   ├─ObjectTypePropertyExpression
//@[09:069) |   | └─ResourceDerivedTypeExpression { Name = Microsoft.Storage/storageAccounts }
  test3: resource</*    */'Microsoft.Storage/storageAccounts@2023-01-01'/*     */>
//@[02:082) |   └─ObjectTypePropertyExpression
//@[09:082) |     └─ResourceDerivedTypeExpression { Name = Microsoft.Storage/storageAccounts }
}

@description('I love space(s)')
//@[00:115) ├─DeclaredTypeExpression { Name = test2 }
//@[13:030) | ├─StringLiteralExpression { Value = I love space(s) }
type test2 = resource<
//@[13:083) | └─ResourceDerivedTypeExpression { Name = Astronomer.Astro/organizations }

     'Astronomer.Astro/organizations@2023-08-01-preview'

>

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

