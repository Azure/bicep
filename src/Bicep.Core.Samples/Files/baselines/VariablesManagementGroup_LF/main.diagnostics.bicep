targetScope='managementGroup'

var deploymentLocation = deployment().location
//@[04:22) [no-unused-vars (Warning)] Variable "deploymentLocation" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |deploymentLocation|
//@[25:46) [no-loc-expr-outside-params (Warning)] Use a parameter here instead of 'deployment().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-loc-expr-outside-params)) |deployment().location|

var scopesWithArmRepresentation = {
//@[04:31) [no-unused-vars (Warning)] Variable "scopesWithArmRepresentation" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |scopesWithArmRepresentation|
  tenant: tenant()
  managementGroup: managementGroup()
}

