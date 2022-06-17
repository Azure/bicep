targetScope='managementGroup'

var deploymentLocation = deployment().location
//@[12:12]     "deploymentLocation": "[deployment().location]",

var scopesWithArmRepresentation = {
//@[13:16]     "scopesWithArmRepresentation": {
  tenant: tenant()
//@[14:14]       "tenant": "[tenant()]",
  managementGroup: managementGroup()
//@[15:15]       "managementGroup": "[managementGroup()]"
}

