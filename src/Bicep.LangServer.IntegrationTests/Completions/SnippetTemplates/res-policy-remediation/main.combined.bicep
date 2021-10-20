// $1 = policyRemediation
// $2 = 'name'
// $3 = 'policyAssignmentId'
// $4 = 'policyDefinitionReferenceId'
// $5 = 'ExistingNonCompliant'
// $6 = 'location'

resource policyRemediation policyRemediation 'Microsoft.PolicyInsights/remediations@2019-07-01' = {
//@[27:44) [BCP068 (Error)] Expected a resource type string. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |policyRemediation|
//@[27:99) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |policyRemediation 'Microsoft.PolicyInsights/remediations@2019-07-01' = {|
//@[99:99) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||
  name: 'name' 'name'
//@[2:6) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |name|
  properties: {
//@[2:12) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |properties|
    policyAssignmentId: 'policyAssignmentId' 'policyAssignmentId'
//@[4:22) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |policyAssignmentId|
    policyDefinitionReferenceId: 'policyDefinitionReferenceId' 'policyDefinitionReferenceId'
//@[4:31) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |policyDefinitionReferenceId|
    resourceDiscoveryMode: 'ExistingNonCompliant' 'ExistingNonCompliant'
//@[4:25) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |resourceDiscoveryMode|
    filters: {
//@[4:11) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |filters|
      locations: [
//@[6:15) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |locations|
        'location'
//@[8:18) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |'location'|
      ]
//@[6:7) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |]|
    }
//@[4:5) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
  }
//@[2:3) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
}
//@[0:1) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
// Insert snippet here

