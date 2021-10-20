targetScope='managementGroup'

var deploymentLocation = deployment().location
//@[4:22) [no-unused-vars (Warning)] Variable "deploymentLocation" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |deploymentLocation|
//@[25:46) [no-hardcoded-location (Warning)] Use a parameter named `location` here instead of 'deployment().location'. 'deployment().location' should only be used as a default for parameter `location`. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |deployment().location|

var scopesWithArmRepresentation = {
//@[4:31) [no-unused-vars (Warning)] Variable "scopesWithArmRepresentation" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |scopesWithArmRepresentation|
  tenant: tenant()
  managementGroup: managementGroup()
}

