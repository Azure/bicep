targetScope='subscription'

var deploymentLocation = deployment().location
//@[11:11]     "deploymentLocation": "[deployment().location]",

var scopesWithArmRepresentation = {
//@[12:15]     "scopesWithArmRepresentation": {
  tenant: tenant()
  subscription: subscription()
}

