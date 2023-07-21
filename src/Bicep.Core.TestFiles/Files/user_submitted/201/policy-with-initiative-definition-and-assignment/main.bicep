targetScope = 'subscription'

param listOfAllowedLocations array = [
  'eastus'
  'eastus2'
  'westus'
  'westus2'
]

param listOfAllowedSKUs array = [
  'Standard_B1ls'
  'Standard_B1ms'
  'Standard_B1s'
  'Standard_B2ms'
  'Standard_B2s'
  'Standard_B4ms'
  'Standard_B4s'
  'Standard_D2s_v3'
  'Standard_D4s_v3'
]

var initiativeDefinitionName = 'BICEP Example Initiative'

resource initiativeDefinition 'Microsoft.Authorization/policySetDefinitions@2019-09-01' = {
  name: initiativeDefinitionName
  properties: {
    policyType: 'Custom'
    displayName: initiativeDefinitionName
    description: 'Initiative Definition for Resource Locatoin and VM SKUs'
    metadata: {
      category: 'BICEP Example Initiative'
    }
    parameters: {
      listOfAllowedLocations: {
        type: 'Array'
        metadata: ({
          description: 'The List of Allowed Locations for Resource Groups and Resources.'
          strongtype: 'location'
          displayName: 'Allowed Locations'
        })
      }
      listOfAllowedSKUs: {
        type: 'Array'
        metadata: any({
          description: 'The List of Allowed SKUs for Virtual Machines.'
          strongtype: 'vmSKUs'
          displayName: 'Allowed Virtual Machine Size SKUs'
        })
      }
    }
    policyDefinitions: [
      {
        policyDefinitionId: '/providers/Microsoft.Authorization/policyDefinitions/e765b5de-1225-4ba3-bd56-1ac6695af988'
        parameters: {
          listOfAllowedLocations: {
            value: '[parameters(\'listOfAllowedLocations\')]'
          }
        }
      }
      {
        policyDefinitionId: '/providers/Microsoft.Authorization/policyDefinitions/e56962a6-4747-49cd-b67b-bf8b01975c4c'
        parameters: {
          listOfAllowedLocations: {
            value: '[parameters(\'listOfAllowedLocations\')]'
          }
        }
      }
      {
        policyDefinitionId: '/providers/Microsoft.Authorization/policyDefinitions/cccc23c7-8427-4f53-ad12-b6a63eb452b3'
        parameters: {
          listOfAllowedSKUs: {
            value: '[parameters(\'listOfAllowedSKUs\')]'
          }
        }
      }
      {
        policyDefinitionId: '/providers/Microsoft.Authorization/policyDefinitions/0015ea4d-51ff-4ce3-8d8c-f3f8f0179a56'
        parameters: {}
      }
    ]
  }
}

resource initiativeDefinitionPolicyAssignment 'Microsoft.Authorization/policyAssignments@2019-09-01' = {
  name: initiativeDefinitionName
  properties: {
    scope: subscription().id
    enforcementMode: 'Default'
    policyDefinitionId: initiativeDefinition.id
    parameters: {
      listOfAllowedLocations: {
        value: listOfAllowedLocations
      }
      listOfAllowedSKUs: {
        value: listOfAllowedSKUs
      }
    }
  }
}
