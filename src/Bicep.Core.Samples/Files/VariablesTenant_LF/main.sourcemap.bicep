targetScope='tenant'

var deploymentLocation = deployment().location
//@[line2->line11]     "deploymentLocation": "[deployment().location]",

var scopesWithArmRepresentation = {
//@[line4->line12]     "scopesWithArmRepresentation": {
//@[line4->line14]     }
  tenant: tenant()
//@[line5->line13]       "tenant": "[tenant()]"
}

