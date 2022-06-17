targetScope='subscription'

var deploymentLocation = deployment().location
//@[12:12]     "deploymentLocation": "[deployment().location]",

var scopesWithArmRepresentation = {
//@[13:16]     "scopesWithArmRepresentation": {
  tenant: tenant()
//@[14:14]       "tenant": "[tenant()]",
  subscription: subscription()
//@[15:15]       "subscription": "[subscription()]"
}

