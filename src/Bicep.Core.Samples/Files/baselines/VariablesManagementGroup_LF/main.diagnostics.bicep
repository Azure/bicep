targetScope='managementGroup'

var deploymentLocation = deployment().location
//@[4:22) [no-unused-vars (Warning)] Variable "deploymentLocation" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |deploymentLocation|

var scopesWithArmRepresentation = {
//@[4:31) [no-unused-vars (Warning)] Variable "scopesWithArmRepresentation" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |scopesWithArmRepresentation|
  tenant: tenant()
  managementGroup: managementGroup()
}

