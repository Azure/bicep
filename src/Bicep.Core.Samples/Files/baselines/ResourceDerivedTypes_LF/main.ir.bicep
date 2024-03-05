type foo = resource<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@[00:1215) ProgramExpression
//@[00:0072) ├─DeclaredTypeExpression { Name = foo }
//@[11:0072) | └─TypeReferencePropertyAccessExpression { Name = string }
//@[11:0067) |   └─ResourceDerivedTypeExpression { Name = Microsoft.Storage/storageAccounts }

type test = {
//@[00:0254) ├─DeclaredTypeExpression { Name = test }
//@[12:0254) | └─ObjectTypeExpression { Name = { resA: string, resB: string, resC: array, resD: string } }
  resA: resource<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@[02:0069) |   ├─ObjectTypePropertyExpression
//@[08:0069) |   | └─TypeReferencePropertyAccessExpression { Name = string }
//@[08:0064) |   |   └─ResourceDerivedTypeExpression { Name = Microsoft.Storage/storageAccounts }
  resB: sys.resource<'Microsoft.Storage/storageAccounts@2022-09-01'>.name
//@[02:0073) |   ├─ObjectTypePropertyExpression
//@[08:0073) |   | └─TypeReferencePropertyAccessExpression { Name = string }
//@[08:0068) |   |   └─ResourceDerivedTypeExpression { Name = Microsoft.Storage/storageAccounts }
  resC: sys.array
//@[02:0017) |   ├─ObjectTypePropertyExpression
//@[08:0017) |   | └─FullyQualifiedAmbientTypeReferenceExpression { Name = sys.array }
  resD: sys.resource<'az:Microsoft.Storage/storageAccounts@2022-09-01'>.name
//@[02:0076) |   └─ObjectTypePropertyExpression
//@[08:0076) |     └─TypeReferencePropertyAccessExpression { Name = string }
//@[08:0071) |       └─ResourceDerivedTypeExpression { Name = Microsoft.Storage/storageAccounts }
}

type strangeFormattings = {
//@[00:0273) ├─DeclaredTypeExpression { Name = strangeFormattings }
//@[26:0273) | └─ObjectTypeExpression { Name = { test: string, test2: string, test3: string } }
  test: resource<
//@[02:0080) |   ├─ObjectTypePropertyExpression
//@[08:0080) |   | └─TypeReferencePropertyAccessExpression { Name = string }
//@[08:0075) |   |   └─ResourceDerivedTypeExpression { Name = Astronomer.Astro/organizations }

  'Astronomer.Astro/organizations@2023-08-01-preview'

>.name
  test2: resource    <'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@[02:0074) |   ├─ObjectTypePropertyExpression
//@[09:0074) |   | └─TypeReferencePropertyAccessExpression { Name = string }
//@[09:0069) |   |   └─ResourceDerivedTypeExpression { Name = Microsoft.Storage/storageAccounts }
  test3: resource</*    */'Microsoft.Storage/storageAccounts@2023-01-01'/*     */>.name
//@[02:0087) |   └─ObjectTypePropertyExpression
//@[09:0087) |     └─TypeReferencePropertyAccessExpression { Name = string }
//@[09:0082) |       └─ResourceDerivedTypeExpression { Name = Microsoft.Storage/storageAccounts }
}

@description('I love space(s)')
//@[00:0120) ├─DeclaredTypeExpression { Name = test2 }
//@[13:0030) | ├─StringLiteralExpression { Value = I love space(s) }
type test2 = resource<
//@[13:0088) | └─TypeReferencePropertyAccessExpression { Name = string }
//@[13:0083) |   └─ResourceDerivedTypeExpression { Name = Astronomer.Astro/organizations }

     'Astronomer.Astro/organizations@2023-08-01-preview'

>.name

param bar resource<'Microsoft.Resources/tags@2022-09-01'>.properties = {
//@[00:0125) ├─DeclaredParameterExpression { Name = bar }
//@[10:0068) | ├─TypeReferencePropertyAccessExpression { Name = Tags }
//@[10:0057) | | └─ResourceDerivedTypeExpression { Name = Microsoft.Resources/tags }
//@[71:0125) | └─ObjectExpression
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

output baz resource<'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31'>.name = 'myId'
//@[00:0096) └─DeclaredOutputExpression { Name = baz }
//@[11:0087)   ├─TypeReferencePropertyAccessExpression { Name = string }
//@[11:0082)   | └─ResourceDerivedTypeExpression { Name = Microsoft.ManagedIdentity/userAssignedIdentities }
//@[90:0096)   └─StringLiteralExpression { Value = myId }

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

