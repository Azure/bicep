targetScope='subscription'

var deploymentLocation = deployment().location
//@[11:11]     "deploymentLocation": "[deployment().location]",\r

var scopesWithArmRepresentation = {
//@[12:15]     "scopesWithArmRepresentation": {\r
  tenant: tenant()
//@[13:13]       "tenant": "[tenant()]",\r
  subscription: subscription()
//@[14:14]       "subscription": "[subscription()]"\r
}

