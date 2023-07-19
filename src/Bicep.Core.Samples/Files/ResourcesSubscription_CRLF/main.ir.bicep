targetScope = 'subscription'
//@[000:1017) ProgramExpression
//@[000:0000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:0000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:0000)   ├─ResourceDependencyExpression [UNPARENTED]
//@[000:0000)   | └─ResourceReferenceExpression [UNPARENTED]
//@[000:0000)   └─ResourceDependencyExpression [UNPARENTED]
//@[000:0000)     └─ResourceReferenceExpression [UNPARENTED]

param ownerPrincipalId string
//@[000:0029) ├─DeclaredParameterExpression { Name = ownerPrincipalId }
//@[023:0029) | └─AmbientTypeReferenceExpression { Name = string }

param contributorPrincipals array
//@[000:0033) ├─DeclaredParameterExpression { Name = contributorPrincipals }
//@[028:0033) | └─AmbientTypeReferenceExpression { Name = array }
param readerPrincipals array
//@[000:0028) ├─DeclaredParameterExpression { Name = readerPrincipals }
//@[023:0028) | └─AmbientTypeReferenceExpression { Name = array }

resource owner 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
//@[000:0242) ├─DeclaredResourceExpression
//@[078:0242) | └─ObjectExpression
  name: guid('owner', ownerPrincipalId)
  properties: {
//@[002:0117) |   └─ObjectPropertyExpression
//@[002:0012) |     ├─StringLiteralExpression { Value = properties }
//@[014:0117) |     └─ObjectExpression
    principalId: ownerPrincipalId
//@[004:0033) |       ├─ObjectPropertyExpression
//@[004:0015) |       | ├─StringLiteralExpression { Value = principalId }
//@[017:0033) |       | └─ParametersReferenceExpression { Parameter = ownerPrincipalId }
    roleDefinitionId: '8e3af657-a8ff-443c-a75c-2fe8c4bcb635'
//@[004:0060) |       └─ObjectPropertyExpression
//@[004:0020) |         ├─StringLiteralExpression { Value = roleDefinitionId }
//@[022:0060) |         └─StringLiteralExpression { Value = 8e3af657-a8ff-443c-a75c-2fe8c4bcb635 }
  }
}

resource contributors 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = [for contributor in contributorPrincipals: {
//@[000:0321) ├─DeclaredResourceExpression
//@[085:0321) | ├─ForLoopExpression
//@[105:0126) | | ├─ParametersReferenceExpression { Parameter = contributorPrincipals }
//@[128:0320) | | └─ObjectExpression
//@[105:0126) | |       |   └─ParametersReferenceExpression { Parameter = contributorPrincipals }
  name: guid('contributor', contributor)
  properties: {
//@[002:0112) | |   └─ObjectPropertyExpression
//@[002:0012) | |     ├─StringLiteralExpression { Value = properties }
//@[014:0112) | |     └─ObjectExpression
    principalId: contributor
//@[004:0028) | |       ├─ObjectPropertyExpression
//@[004:0015) | |       | ├─StringLiteralExpression { Value = principalId }
//@[017:0028) | |       | └─ArrayAccessExpression
//@[017:0028) | |       |   ├─CopyIndexExpression
    roleDefinitionId: 'b24988ac-6180-42a0-ab88-20f7382dd24c'
//@[004:0060) | |       └─ObjectPropertyExpression
//@[004:0020) | |         ├─StringLiteralExpression { Value = roleDefinitionId }
//@[022:0060) | |         └─StringLiteralExpression { Value = b24988ac-6180-42a0-ab88-20f7382dd24c }
  }
  dependsOn: [
    owner
  ]
}]

resource readers 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = [for reader in readerPrincipals: {
//@[000:0312) └─DeclaredResourceExpression
//@[080:0312)   ├─ForLoopExpression
//@[095:0111)   | ├─ParametersReferenceExpression { Parameter = readerPrincipals }
//@[113:0311)   | └─ObjectExpression
//@[095:0111)   |       |   └─ParametersReferenceExpression { Parameter = readerPrincipals }
  name: guid('reader', reader)
  properties: {
//@[002:0107)   |   └─ObjectPropertyExpression
//@[002:0012)   |     ├─StringLiteralExpression { Value = properties }
//@[014:0107)   |     └─ObjectExpression
    principalId: reader
//@[004:0023)   |       ├─ObjectPropertyExpression
//@[004:0015)   |       | ├─StringLiteralExpression { Value = principalId }
//@[017:0023)   |       | └─ArrayAccessExpression
//@[017:0023)   |       |   ├─CopyIndexExpression
    roleDefinitionId: 'b24988ac-6180-42a0-ab88-20f7382dd24c'
//@[004:0060)   |       └─ObjectPropertyExpression
//@[004:0020)   |         ├─StringLiteralExpression { Value = roleDefinitionId }
//@[022:0060)   |         └─StringLiteralExpression { Value = b24988ac-6180-42a0-ab88-20f7382dd24c }
  }
  dependsOn: [
    owner
    contributors[0]
  ]
}]

