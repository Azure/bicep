targetScope='tenant'

var deploymentLocation = deployment().location
//@[11:11]     "deploymentLocation": "[deployment().location]",

var scopesWithArmRepresentation = {
//@[12:14]     "scopesWithArmRepresentation": {
  tenant: tenant()
}

