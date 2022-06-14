targetScope='subscription'

var deploymentLocation = deployment().location
//@[11:11]     "deploymentLocation": "[deployment().location]",

var scopesWithArmRepresentation = {
//@[12:15]     "scopesWithArmRepresentation": {
  tenant: tenant()
//@[13:13]       "tenant": "[tenant()]",
  subscription: subscription()
//@[14:14]       "subscription": "[subscription()]"
}

