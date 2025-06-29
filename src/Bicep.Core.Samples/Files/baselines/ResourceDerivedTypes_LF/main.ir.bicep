type foo = resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@[00:1279) ProgramExpression
//@[00:0077) ├─DeclaredTypeExpression { Name = foo }
//@[11:0077) | └─TypeReferencePropertyAccessExpression { Name = string }
//@[11:0072) |   └─ResourceDerivedTypeExpression { Name = Microsoft.Storage/storageAccounts }

type test = {
//@[00:0269) ├─DeclaredTypeExpression { Name = test }
//@[12:0269) | └─ObjectTypeExpression { Name = { resA: string, resB: string, resC: array, resD: string } }
  resA: resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@[02:0074) |   ├─ObjectTypePropertyExpression
//@[08:0074) |   | └─TypeReferencePropertyAccessExpression { Name = string }
//@[08:0069) |   |   └─ResourceDerivedTypeExpression { Name = Microsoft.Storage/storageAccounts }
  resB: sys.resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>.name
//@[02:0078) |   ├─ObjectTypePropertyExpression
//@[08:0078) |   | └─TypeReferencePropertyAccessExpression { Name = string }
//@[08:0073) |   |   └─ResourceDerivedTypeExpression { Name = Microsoft.Storage/storageAccounts }
  resC: sys.array
//@[02:0017) |   ├─ObjectTypePropertyExpression
//@[08:0017) |   | └─FullyQualifiedAmbientTypeReferenceExpression { Name = sys.array }
  resD: sys.resourceInput<'az:Microsoft.Storage/storageAccounts@2022-09-01'>.name
//@[02:0081) |   └─ObjectTypePropertyExpression
//@[08:0081) |     └─TypeReferencePropertyAccessExpression { Name = string }
//@[08:0076) |       └─ResourceDerivedTypeExpression { Name = Microsoft.Storage/storageAccounts }
}

type strangeFormatting = {
//@[00:0287) ├─DeclaredTypeExpression { Name = strangeFormatting }
//@[25:0287) | └─ObjectTypeExpression { Name = { test: string, test2: string, test3: string } }
  test: resourceInput<
//@[02:0085) |   ├─ObjectTypePropertyExpression
//@[08:0085) |   | └─TypeReferencePropertyAccessExpression { Name = string }
//@[08:0080) |   |   └─ResourceDerivedTypeExpression { Name = Astronomer.Astro/organizations }

  'Astronomer.Astro/organizations@2023-08-01-preview'

>.name
  test2: resourceInput    <'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@[02:0079) |   ├─ObjectTypePropertyExpression
//@[09:0079) |   | └─TypeReferencePropertyAccessExpression { Name = string }
//@[09:0074) |   |   └─ResourceDerivedTypeExpression { Name = Microsoft.Storage/storageAccounts }
  test3: resourceInput</*    */'Microsoft.Storage/storageAccounts@2023-01-01'/*     */>.name
//@[02:0092) |   └─ObjectTypePropertyExpression
//@[09:0092) |     └─TypeReferencePropertyAccessExpression { Name = string }
//@[09:0087) |       └─ResourceDerivedTypeExpression { Name = Microsoft.Storage/storageAccounts }
}

@description('I love space(s)')
//@[00:0125) ├─DeclaredTypeExpression { Name = test2 }
//@[13:0030) | ├─StringLiteralExpression { Value = I love space(s) }
type test2 = resourceInput<
//@[13:0093) | └─TypeReferencePropertyAccessExpression { Name = string }
//@[13:0088) |   └─ResourceDerivedTypeExpression { Name = Astronomer.Astro/organizations }

     'Astronomer.Astro/organizations@2023-08-01-preview'

>.name

param bar resourceInput<'Microsoft.Resources/tags@2022-09-01'>.properties = {
//@[00:0130) ├─DeclaredParameterExpression { Name = bar }
//@[10:0073) | ├─TypeReferencePropertyAccessExpression { Name = Tags }
//@[10:0062) | | └─ResourceDerivedTypeExpression { Name = Microsoft.Resources/tags }
//@[76:0130) | └─ObjectExpression
  tags: {
//@[02:0050) |   └─ObjectPropertyExpression
//@[02:0006) |     ├─StringLiteralExpression { Value = tags }
//@[08:0050) |     └─ObjectExpression
    fizz: 'buzz'
//@[04:0016) |       ├─ObjectPropertyExpression
//@[04:0008) |       | ├─StringLiteralExpression { Value = fizz }
//@[10:0016) |       | └─StringLiteralExpression { Value = buzz }
    snap: 'crackle'
//@[04:0019) |       └─ObjectPropertyExpression
//@[04:0008) |         ├─StringLiteralExpression { Value = snap }
//@[10:0019) |         └─StringLiteralExpression { Value = crackle }
  }
}

output baz resourceInput<'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31'>.name = 'myId'
//@[00:0101) └─DeclaredOutputExpression { Name = baz }
//@[11:0092)   ├─TypeReferencePropertyAccessExpression { Name = string }
//@[11:0087)   | └─ResourceDerivedTypeExpression { Name = Microsoft.ManagedIdentity/userAssignedIdentities }
//@[95:0101)   └─StringLiteralExpression { Value = myId }

type storageAccountName = resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@[00:0092) ├─DeclaredTypeExpression { Name = storageAccountName }
//@[26:0092) | └─TypeReferencePropertyAccessExpression { Name = string }
//@[26:0087) |   └─ResourceDerivedTypeExpression { Name = Microsoft.Storage/storageAccounts }
type accessPolicy = resourceInput<'Microsoft.KeyVault/vaults@2022-07-01'>.properties.accessPolicies[*]
//@[00:0102) ├─DeclaredTypeExpression { Name = accessPolicy }
//@[20:0102) | └─TypeReferenceItemsAccessExpression { Name = AccessPolicyEntry }
//@[20:0099) |   └─TypeReferencePropertyAccessExpression { Name = AccessPolicyEntry[] }
//@[20:0084) |     └─TypeReferencePropertyAccessExpression { Name = VaultProperties }
//@[20:0073) |       └─ResourceDerivedTypeExpression { Name = Microsoft.KeyVault/vaults }
type tag = resourceInput<'Microsoft.Resources/tags@2022-09-01'>.properties.tags.*
//@[00:0081) ├─DeclaredTypeExpression { Name = tag }
//@[11:0081) | └─TypeReferenceAdditionalPropertiesAccessExpression { Name = string }
//@[11:0079) |   └─TypeReferencePropertyAccessExpression { Name = Tags }
//@[11:0074) |     └─TypeReferencePropertyAccessExpression { Name = Tags }
//@[11:0063) |       └─ResourceDerivedTypeExpression { Name = Microsoft.Resources/tags }

