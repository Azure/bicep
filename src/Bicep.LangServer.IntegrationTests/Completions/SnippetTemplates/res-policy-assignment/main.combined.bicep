// $1 = policyAssignment
// $2 = 'name'
// $3 = 'location'
// $4 = 'SystemAssigned'
// $5 = 'displayName'
// $6 = 'description'
// $7 = 'Default'
// $8 = 'source'
// $9 = '0.1.0'
// $10 = 'policyDefinitionId'
// $11 = parameterName
// $12 = 'value'
// $13 = 'message'
// $14 = 'message'
// $15 = 'policyDefinitionReferenceId'

param location string
//@[6:14) [no-unused-params (Warning)] Parameter "location" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |location|

resource policyAssignment policyAssignment 'Microsoft.Authorization/policyAssignments@2020-09-01' = {
//@[26:42) [BCP068 (Error)] Expected a resource type string. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |policyAssignment|
//@[26:101) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |policyAssignment 'Microsoft.Authorization/policyAssignments@2020-09-01' = {|
//@[101:101) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||
  name: 'name' 'name'
//@[2:6) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |name|
  location: location
//@[2:10) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |location|
  identity: {
//@[2:10) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |identity|
    type: 'location' 'SystemAssigned'
//@[4:8) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |type|
  }
//@[2:3) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
  properties: {
//@[2:12) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |properties|
    displayName: 'SystemAssigned' 'displayName'
//@[4:15) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |displayName|
    description: 'displayName' 'description'
//@[4:15) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |description|
    enforcementMode: 'description' 'Default'
//@[4:19) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |enforcementMode|
    metadata: {
//@[4:12) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |metadata|
      source: 'Default' 'source'
//@[6:12) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |source|
      version: 'source' '0.1.0'
//@[6:13) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |version|
    }
//@[4:5) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
    policyDefinitionId: '0.1.0' 'policyDefinitionId'
//@[4:22) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |policyDefinitionId|
    parameters: {
//@[4:14) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |parameters|
      'policyDefinitionId': {
//@[6:26) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |'policyDefinitionId'|
        value: parameterName 'value'
//@[8:13) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |value|
      }
//@[6:7) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
    }
//@[4:5) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
    nonComplianceMessages: [
//@[4:25) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |nonComplianceMessages|
      {
//@[6:7) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |{|
        message: 'value' 'message'
//@[8:15) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |message|
      }
//@[6:7) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
      {
//@[6:7) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |{|
        message: 'message' 'message'
//@[8:15) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |message|
        policyDefinitionReferenceId: 'message' 'policyDefinitionReferenceId'
//@[8:35) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |policyDefinitionReferenceId|
      }
//@[6:7) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
    ]
//@[4:5) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |]|
  }
//@[2:3) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
}
//@[0:1) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
// Insert snippet here

