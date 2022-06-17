targetScope='tenant'

var deploymentLocation = deployment().location
//@[12:12]     "deploymentLocation": "[deployment().location]",

var scopesWithArmRepresentation = {
//@[13:15]     "scopesWithArmRepresentation": {
  tenant: tenant()
//@[14:14]       "tenant": "[tenant()]"
}

