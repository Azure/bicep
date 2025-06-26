targetScope='subscription'

var deploymentLocation = deployment().location
//@[4:22) [no-unused-vars (Warning)] Variable "deploymentLocation" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |deploymentLocation|

var scopesWithArmRepresentation = {
//@[4:31) [no-unused-vars (Warning)] Variable "scopesWithArmRepresentation" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |scopesWithArmRepresentation|
  tenant: tenant()
  subscription: subscription()
}

