// REP 0015: the classic "hard" scope expression. A conditional (ternary) scope that today fails with
//@[00:554) ProgramExpression
// BCP420 ("scope could not be resolved at compile time") now compiles: both branches are ResourceScope
// members sharing the 'resourceGroup' discriminator, so the whole expression is emitted verbatim into
// "@scope" and the deployment engine resolves it at deploy time.
param otherResourceGroup string = ''
//@[00:036) ├─DeclaredParameterExpression { Name = otherResourceGroup }
//@[25:031) | ├─AmbientTypeReferenceExpression { Name = string }
//@[34:036) | └─StringLiteralExpression { Value =  }

module mod 'modules/mod.bicep' = {
//@[00:140) └─DeclaredModuleExpression
//@[33:140)   └─ObjectExpression
  name: 'mod'
//@[02:013)     └─ObjectPropertyExpression
//@[02:006)       ├─StringLiteralExpression { Value = name }
//@[08:013)       └─StringLiteralExpression { Value = mod }
  scope: !empty(otherResourceGroup) ? resourceGroup(otherResourceGroup) : resourceGroup()
}

