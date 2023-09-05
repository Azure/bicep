targetScope='managementGroup'

var deploymentLocation = deployment().location
//@    "deploymentLocation": "[deployment().location]",

var scopesWithArmRepresentation = {
//@    "scopesWithArmRepresentation": {
//@    }
  tenant: tenant()
//@      "tenant": "[tenant()]",
  managementGroup: managementGroup()
//@      "managementGroup": "[managementGroup()]"
}

