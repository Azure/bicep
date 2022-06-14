targetScope='tenant'

var deploymentLocation = deployment().location
//@[11:11]     "deploymentLocation": "[deployment().location]",\r

var scopesWithArmRepresentation = {
//@[12:14]     "scopesWithArmRepresentation": {\r
  tenant: tenant()
//@[13:13]       "tenant": "[tenant()]"\r
}

