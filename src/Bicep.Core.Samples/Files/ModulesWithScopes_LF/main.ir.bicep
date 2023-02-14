targetScope = 'tenant'
//@[000:1047) ProgramExpression

module myManagementGroupMod 'modules/managementgroup.bicep' = {
//@[000:0142) ├─DeclaredModuleExpression
//@[062:0142) | └─ObjectExpression
  name: 'myManagementGroupMod'
//@[002:0030) |   └─ObjectPropertyExpression
//@[002:0006) |     ├─StringLiteralExpression { Value = name }
//@[008:0030) |     └─StringLiteralExpression { Value = myManagementGroupMod }
  scope: managementGroup('myManagementGroup')
}
module myManagementGroupModWithDuplicatedNameButDifferentScope 'modules/managementgroup_empty.bicep' = {
//@[000:0184) ├─DeclaredModuleExpression
//@[103:0184) | └─ObjectExpression
  name: 'myManagementGroupMod'
//@[002:0030) |   └─ObjectPropertyExpression
//@[002:0006) |     ├─StringLiteralExpression { Value = name }
//@[008:0030) |     └─StringLiteralExpression { Value = myManagementGroupMod }
  scope: managementGroup('myManagementGroup2')
}
module mySubscriptionMod 'modules/subscription.bicep' = {
//@[000:0149) ├─DeclaredModuleExpression
//@[056:0149) | └─ObjectExpression
  name: 'mySubscriptionMod'
//@[002:0027) |   └─ObjectPropertyExpression
//@[002:0006) |     ├─StringLiteralExpression { Value = name }
//@[008:0027) |     └─StringLiteralExpression { Value = mySubscriptionMod }
  scope: subscription('ee44cd78-68c6-43d9-874e-e684ec8d1191')
}

module mySubscriptionModWithCondition 'modules/subscription.bicep' = if (length('foo') == 3) {
//@[000:0199) ├─DeclaredModuleExpression
//@[073:0091) | └─ConditionExpression
//@[073:0091) |   ├─BinaryExpression { Operator = Equals }
//@[073:0086) |   | ├─FunctionCallExpression { Name = length }
//@[080:0085) |   | | └─StringLiteralExpression { Value = foo }
//@[090:0091) |   | └─IntegerLiteralExpression { Value = 3 }
//@[093:0199) |   └─ObjectExpression
  name: 'mySubscriptionModWithCondition'
//@[002:0040) |     └─ObjectPropertyExpression
//@[002:0006) |       ├─StringLiteralExpression { Value = name }
//@[008:0040) |       └─StringLiteralExpression { Value = mySubscriptionModWithCondition }
  scope: subscription('ee44cd78-68c6-43d9-874e-e684ec8d1191')
}

module mySubscriptionModWithDuplicatedNameButDifferentScope 'modules/subscription_empty.bicep' = {
//@[000:0190) ├─DeclaredModuleExpression
//@[097:0190) | └─ObjectExpression
  name: 'mySubscriptionMod'
//@[002:0027) |   └─ObjectPropertyExpression
//@[002:0006) |     ├─StringLiteralExpression { Value = name }
//@[008:0027) |     └─StringLiteralExpression { Value = mySubscriptionMod }
  scope: subscription('1ad827ac-2669-4c2f-9970-282b93c3c550')
}


output myManagementGroupOutput string = myManagementGroupMod.outputs.myOutput
//@[000:0077) ├─DeclaredOutputExpression { Name = myManagementGroupOutput }
//@[040:0077) | └─ModuleOutputPropertyAccessExpression { PropertyName = myOutput }
//@[040:0068) |   └─PropertyAccessExpression { PropertyName = outputs }
//@[040:0060) |     └─ModuleReferenceExpression
output mySubscriptionOutput string = mySubscriptionMod.outputs.myOutput
//@[000:0071) └─DeclaredOutputExpression { Name = mySubscriptionOutput }
//@[037:0071)   └─ModuleOutputPropertyAccessExpression { PropertyName = myOutput }
//@[037:0062)     └─PropertyAccessExpression { PropertyName = outputs }
//@[037:0054)       └─ModuleReferenceExpression

