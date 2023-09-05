targetScope='tenant'
//@[00:127) ProgramExpression

var deploymentLocation = deployment().location
//@[00:046) ├─DeclaredVariableExpression { Name = deploymentLocation }
//@[25:046) | └─PropertyAccessExpression { PropertyName = location }
//@[25:037) |   └─FunctionCallExpression { Name = deployment }

var scopesWithArmRepresentation = {
//@[00:056) └─DeclaredVariableExpression { Name = scopesWithArmRepresentation }
//@[34:056)   └─ObjectExpression
  tenant: tenant()
//@[02:018)     └─ObjectPropertyExpression
//@[02:008)       ├─StringLiteralExpression { Value = tenant }
//@[10:018)       └─FunctionCallExpression { Name = tenant }
}

