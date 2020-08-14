input string rgName
input string rgLocation
input string principalId
input string roleDefinitionId: 'b24988ac-6180-42a0-ab88-20f7382dd24c'
input string roleAssignmentName: '${rgName}-manager'
input string policyName

variable subScope: subscriptionScope() // TODO need to think about how to pick the right scope

scope subScope { // TODO is this syntax confusing?
  resource 'resources/resourceGroups@2018-05-01' containerRg {
    scope: subScope
    name: rgName
    location: rgLocation
    tags: {
      note: 'this group was created by Contoso IT'
    }
    properties: {}
  }
}

variable rgScope: containerRg.scope() // TODO allow utilization of newly-created resource group scope?

scope rgScope {
  resource 'authorization/locks@2017-04-01' lock {
    name: 'DontDelete'
    properties: {
      level: 'CanNotDelete'
      notes: 'Prevent deletion of the resourceGroup'
    }
  }

  resource 'authorization/roleAssignments@2017-05-01' assignment {
    name: guid(roleAssignmentName)
    properties: {
      scope: rgScope.id()
      roleDefinitionId: '${subscription().id}/providers/Microsoft.Authorization/roleDefinitions/${roleDefinitionId}'
      principalId: principalId
    }
  }

  resource 'authorization/policyAssignments@2018-03-01' policyAssignment {
    name: 'LockRGLocations'
    properties: {
      scope: rgScope.id()
      policyDefinitionId: '${subscription().id}/providers/Microsoft.Authorization/policyDefinitions/${policyName}'
    }
  }
}

output test: roleAssignmentName