type foo = resource<'Microsoft.Storage/storageAccounts@2023-01-01'>
//@[00:1238) ProgramExpression
//@[00:0067) ├─DeclaredTypeExpression { Name = foo }
//@[11:0067) | └─ResourceDerivedTypeExpression { Name = Microsoft.Storage/storageAccounts }

type test = {
//@[00:0239) ├─DeclaredTypeExpression { Name = test }
//@[12:0239) | └─ObjectTypeExpression { Name = { resA: Microsoft.Storage/storageAccounts, resB: Microsoft.Storage/storageAccounts, resC: array, resD: Microsoft.Storage/storageAccounts } }
  resA: resource<'Microsoft.Storage/storageAccounts@2023-01-01'>
//@[02:0064) |   ├─ObjectTypePropertyExpression
//@[08:0064) |   | └─ResourceDerivedTypeExpression { Name = Microsoft.Storage/storageAccounts }
  resB: sys.resource<'Microsoft.Storage/storageAccounts@2022-09-01'>
//@[02:0068) |   ├─ObjectTypePropertyExpression
//@[08:0068) |   | └─ResourceDerivedTypeExpression { Name = Microsoft.Storage/storageAccounts }
  resC: sys.array
//@[02:0017) |   ├─ObjectTypePropertyExpression
//@[08:0017) |   | └─FullyQualifiedAmbientTypeReferenceExpression { Name = sys.array }
  resD: sys.resource<'az:Microsoft.Storage/storageAccounts@2022-09-01'>
//@[02:0071) |   └─ObjectTypePropertyExpression
//@[08:0071) |     └─ResourceDerivedTypeExpression { Name = Microsoft.Storage/storageAccounts }
}

type strangeFormattings = {
//@[00:0258) ├─DeclaredTypeExpression { Name = strangeFormattings }
//@[26:0258) | └─ObjectTypeExpression { Name = { test: Astronomer.Astro/organizations, test2: Microsoft.Storage/storageAccounts, test3: Microsoft.Storage/storageAccounts } }
  test: resource<
//@[02:0075) |   ├─ObjectTypePropertyExpression
//@[08:0075) |   | └─ResourceDerivedTypeExpression { Name = Astronomer.Astro/organizations }

  'Astronomer.Astro/organizations@2023-08-01-preview'

>
  test2: resource    <'Microsoft.Storage/storageAccounts@2023-01-01'>
//@[02:0069) |   ├─ObjectTypePropertyExpression
//@[09:0069) |   | └─ResourceDerivedTypeExpression { Name = Microsoft.Storage/storageAccounts }
  test3: resource</*    */'Microsoft.Storage/storageAccounts@2023-01-01'/*     */>
//@[02:0082) |   └─ObjectTypePropertyExpression
//@[09:0082) |     └─ResourceDerivedTypeExpression { Name = Microsoft.Storage/storageAccounts }
}

@description('I love space(s)')
//@[00:0115) ├─DeclaredTypeExpression { Name = test2 }
//@[13:0030) | ├─StringLiteralExpression { Value = I love space(s) }
type test2 = resource<
//@[13:0083) | └─ResourceDerivedTypeExpression { Name = Astronomer.Astro/organizations }

     'Astronomer.Astro/organizations@2023-08-01-preview'

>

param bar resource<'Microsoft.Resources/tags@2022-09-01'> = {
//@[00:0160) ├─DeclaredParameterExpression { Name = bar }
//@[10:0057) | ├─ResourceDerivedTypeExpression { Name = Microsoft.Resources/tags }
//@[60:0160) | └─ObjectExpression
  name: 'default'
//@[02:0017) |   ├─ObjectPropertyExpression
//@[02:0006) |   | ├─StringLiteralExpression { Value = name }
//@[08:0017) |   | └─StringLiteralExpression { Value = default }
  properties: {
//@[02:0078) |   └─ObjectPropertyExpression
//@[02:0012) |     ├─StringLiteralExpression { Value = properties }
//@[14:0078) |     └─ObjectExpression
    tags: {
//@[04:0058) |       └─ObjectPropertyExpression
//@[04:0008) |         ├─StringLiteralExpression { Value = tags }
//@[10:0058) |         └─ObjectExpression
      fizz: 'buzz'
//@[06:0018) |           ├─ObjectPropertyExpression
//@[06:0010) |           | ├─StringLiteralExpression { Value = fizz }
//@[12:0018) |           | └─StringLiteralExpression { Value = buzz }
      snap: 'crackle'
//@[06:0021) |           └─ObjectPropertyExpression
//@[06:0010) |             ├─StringLiteralExpression { Value = snap }
//@[12:0021) |             └─StringLiteralExpression { Value = crackle }
    }
  }
}

output baz resource<'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31'> = {
//@[00:0124) └─DeclaredOutputExpression { Name = baz }
//@[11:0082)   ├─ResourceDerivedTypeExpression { Name = Microsoft.ManagedIdentity/userAssignedIdentities }
//@[85:0124)   └─ObjectExpression
  name: 'myId'
//@[02:0014)     ├─ObjectPropertyExpression
//@[02:0006)     | ├─StringLiteralExpression { Value = name }
//@[08:0014)     | └─StringLiteralExpression { Value = myId }
  location: 'eastus'
//@[02:0020)     └─ObjectPropertyExpression
//@[02:0010)       ├─StringLiteralExpression { Value = location }
//@[12:0020)       └─StringLiteralExpression { Value = eastus }
}

type storageAccountName = resource<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@[00:0087) ├─DeclaredTypeExpression { Name = storageAccountName }
//@[26:0087) | └─TypeReferencePropertyAccessExpression { Name = string }
//@[26:0082) |   └─ResourceDerivedTypeExpression { Name = Microsoft.Storage/storageAccounts }
type accessPolicy = resource<'Microsoft.KeyVault/vaults@2022-07-01'>.properties.accessPolicies[*]
//@[00:0097) ├─DeclaredTypeExpression { Name = accessPolicy }
//@[20:0097) | └─TypeReferenceItemsAccessExpression { Name = AccessPolicyEntry }
//@[20:0094) |   └─TypeReferencePropertyAccessExpression { Name = AccessPolicyEntry[] }
//@[20:0079) |     └─TypeReferencePropertyAccessExpression { Name = VaultProperties }
//@[20:0068) |       └─ResourceDerivedTypeExpression { Name = Microsoft.KeyVault/vaults }
type tag = resource<'Microsoft.Resources/tags@2022-09-01'>.properties.tags.*
//@[00:0076) ├─DeclaredTypeExpression { Name = tag }
//@[11:0076) | └─TypeReferenceAdditionalPropertiesAccessExpression { Name = string }
//@[11:0074) |   └─TypeReferencePropertyAccessExpression { Name = Tags }
//@[11:0069) |     └─TypeReferencePropertyAccessExpression { Name = Tags }
//@[11:0058) |       └─ResourceDerivedTypeExpression { Name = Microsoft.Resources/tags }

