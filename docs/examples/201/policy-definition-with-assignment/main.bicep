targetScope = 'subscription'

param listofAllowedLocations array {
  default: [
    'norwayeast'
    'westeurope'
  ]
}
param policyEffect string {
  allowed:[
    'Audit'
    'Deny'
  ]
}

param assignmentScope string

resource locationPolicyDefinition 'Microsoft.Authorization/policyDefinitions@2020-09-01' = {
  name: 'custom-allowed-location'
  properties:{
    displayName: 'Custom - allowed location for resources'
    policyType: 'Custom'
    description: 'Use policy to restrict where resources can be deployed'
    parameters: {
      allowedLocations: {
        type: 'Array'
      }
      effect: {
        type: 'String'
      }
    }
    metadata: {
      category: 'Locations'
    }
    policyRule: {
      if: {
        allOf: [
          {
            field: 'location'
            notIn: '[parameters(\'allowedLocations\')]'
          }
          {
            field: 'location'
            notEquals: 'global'
          }
          {
            field: 'type'
            notEquals: 'Microsoft.AzureActiveDirectory/b2cDirectories'
          }
      ]
    }
      then: {
        effect: '[parameters(\'effect\')]'
      }
    }
  }
}

resource locationPolicy 'Microsoft.Authorization/policyAssignments@2020-09-01' = {
 name: 'Resource-location-restriction'
 dependsOn: [
   locationPolicyDefinition
 ]
 properties:{
   policyDefinitionId: locationPolicyDefinition.id
   displayName: 'Restrict location for Azure resources'
   description: 'Policy will either Audit or Deny resources being deployed in other locations'
   parameters: {
     allowedLocations: {
       value: listofAllowedLocations
     }
     Effect: {
      value: policyEffect
    }
   }
 }
}