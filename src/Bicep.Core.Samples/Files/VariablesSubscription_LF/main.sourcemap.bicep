targetScope='subscription'

var deploymentLocation = deployment().location
//@[line2->line11]     "deploymentLocation": "[deployment().location]",

var scopesWithArmRepresentation = {
//@[line4->line12]     "scopesWithArmRepresentation": {
//@[line4->line15]     }
  tenant: tenant()
//@[line5->line13]       "tenant": "[tenant()]",
  subscription: subscription()
//@[line6->line14]       "subscription": "[subscription()]"
}

