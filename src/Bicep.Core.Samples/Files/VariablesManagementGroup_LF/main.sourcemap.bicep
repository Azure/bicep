targetScope='managementGroup'

var deploymentLocation = deployment().location
//@[line2->line11]     "deploymentLocation": "[deployment().location]",

var scopesWithArmRepresentation = {
//@[line4->line12]     "scopesWithArmRepresentation": {
//@[line4->line15]     }
  tenant: tenant()
//@[line5->line13]       "tenant": "[tenant()]",
  managementGroup: managementGroup()
//@[line6->line14]       "managementGroup": "[managementGroup()]"
}

