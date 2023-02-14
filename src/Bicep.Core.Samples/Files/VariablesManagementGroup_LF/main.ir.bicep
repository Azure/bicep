targetScope='managementGroup'
//@[00:173) ProgramExpression

var deploymentLocation = deployment().location
//@[00:046) ├─DeclaredVariableExpression { Name = deploymentLocation }
//@[25:046) | └─PropertyAccessExpression { PropertyName = location }
//@[25:037) |   └─FunctionCallExpression { Name = deployment }

var scopesWithArmRepresentation = {
//@[00:093) └─DeclaredVariableExpression { Name = scopesWithArmRepresentation }
//@[34:093)   └─ObjectExpression
  tenant: tenant()
//@[02:018)     ├─ObjectPropertyExpression
//@[02:008)     | ├─StringLiteralExpression { Value = tenant }
//@[10:018)     | └─FunctionCallExpression { Name = tenant }
  managementGroup: managementGroup()
//@[02:036)     └─ObjectPropertyExpression
//@[02:017)       ├─StringLiteralExpression { Value = managementGroup }
//@[19:036)       └─FunctionCallExpression { Name = managementGroup }
}

