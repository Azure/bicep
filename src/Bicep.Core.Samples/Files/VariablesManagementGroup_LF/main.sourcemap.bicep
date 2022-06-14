targetScope='managementGroup'

var deploymentLocation = deployment().location
//@[11:11]     "deploymentLocation": "[deployment().location]",\r

var scopesWithArmRepresentation = {
//@[12:15]     "scopesWithArmRepresentation": {\r
  tenant: tenant()
//@[13:13]       "tenant": "[tenant()]",\r
  managementGroup: managementGroup()
//@[14:14]       "managementGroup": "[managementGroup()]"\r
}

