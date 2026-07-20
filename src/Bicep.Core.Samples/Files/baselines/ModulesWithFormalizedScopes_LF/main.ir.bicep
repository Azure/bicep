targetScope = 'subscription'
//@[00:655) ProgramExpression

param location string = 'eastus'
//@[00:032) ├─DeclaredParameterExpression { Name = location }
//@[15:021) | ├─AmbientTypeReferenceExpression { Name = string }
//@[24:032) | └─StringLiteralExpression { Value = eastus }

// REP 0015: with the 'formalizedScope' experimental feature enabled, this module's cross-scope
// targeting is emitted as a single duck-typed "@scope" object instead of the legacy
// "subscriptionId" / "resourceGroup" properties.
module storageMod 'modules/mod.bicep' = {
//@[00:135) ├─DeclaredModuleExpression
//@[40:135) | ├─ObjectExpression
  name: 'storageMod'
//@[02:020) | | └─ObjectPropertyExpression
//@[02:006) | |   ├─StringLiteralExpression { Value = name }
//@[08:020) | |   └─StringLiteralExpression { Value = storageMod }
  scope: resourceGroup('my-rg')
  params: {
//@[10:038) | └─ObjectExpression
    location: location
//@[04:022) |   └─ObjectPropertyExpression
//@[04:012) |     ├─StringLiteralExpression { Value = location }
//@[14:022) |     └─ParametersReferenceExpression { Parameter = location }
  }
}

module storageMod2 'modules/mod.bicep' = {
//@[00:178) ├─DeclaredModuleExpression
//@[41:178) | ├─ObjectExpression
  name: 'storageMod2'
//@[02:021) | | └─ObjectPropertyExpression
//@[02:006) | |   ├─StringLiteralExpression { Value = name }
//@[08:021) | |   └─StringLiteralExpression { Value = storageMod2 }
  scope: location != 'eastus' ? resourceGroup() : resourceGroup('my-rg')
  params: {
//@[10:038) | └─ObjectExpression
    location: location
//@[04:022) |   └─ObjectPropertyExpression
//@[04:012) |     ├─StringLiteralExpression { Value = location }
//@[14:022) |     └─ParametersReferenceExpression { Parameter = location }
  }
}

output loc string = storageMod.outputs.loc
//@[00:042) └─DeclaredOutputExpression { Name = loc }
//@[11:017)   ├─AmbientTypeReferenceExpression { Name = string }
//@[20:042)   └─ModuleOutputPropertyAccessExpression { PropertyName = loc }
//@[20:038)     └─PropertyAccessExpression { PropertyName = outputs }
//@[20:030)       └─ModuleReferenceExpression

