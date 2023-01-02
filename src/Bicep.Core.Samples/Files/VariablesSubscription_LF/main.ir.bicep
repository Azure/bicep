targetScope='subscription'
//@[00:164) ProgramExpression

var deploymentLocation = deployment().location
//@[00:046) ├─DeclaredVariableExpression { Name = deploymentLocation }
//@[25:046) | └─PropertyAccessExpression { PropertyName = location }
//@[25:037) |   └─FunctionCallExpression { Name = deployment }

var scopesWithArmRepresentation = {
//@[00:087) └─DeclaredVariableExpression { Name = scopesWithArmRepresentation }
//@[34:087)   └─ObjectExpression
  tenant: tenant()
//@[02:018)     ├─ObjectPropertyExpression
//@[02:008)     | ├─StringLiteralExpression { Value = tenant }
//@[10:018)     | └─FunctionCallExpression { Name = tenant }
  subscription: subscription()
//@[02:030)     └─ObjectPropertyExpression
//@[02:014)       ├─StringLiteralExpression { Value = subscription }
//@[16:030)       └─FunctionCallExpression { Name = subscription }
}

