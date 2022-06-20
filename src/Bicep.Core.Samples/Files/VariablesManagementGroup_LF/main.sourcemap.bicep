targetScope='managementGroup'

var deploymentLocation = deployment().location
//@[11:11]     "deploymentLocation": "[deployment().location]",

var scopesWithArmRepresentation = {
//@[12:15]     "scopesWithArmRepresentation": {
  tenant: tenant()
//@[13:13]       "tenant": "[tenant()]",
  managementGroup: managementGroup()
//@[14:14]       "managementGroup": "[managementGroup()]"
}

