targetScope='tenant'

var deploymentLocation = deployment().location
//@    "deploymentLocation": "[deployment().location]",

var scopesWithArmRepresentation = {
//@    "scopesWithArmRepresentation": {
//@    }
  tenant: tenant()
//@      "tenant": "[tenant()]"
}

