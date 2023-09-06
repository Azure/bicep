targetScope='subscription'

var deploymentLocation = deployment().location
//@    "deploymentLocation": "[deployment().location]",

var scopesWithArmRepresentation = {
//@    "scopesWithArmRepresentation": {
//@    }
  tenant: tenant()
//@      "tenant": "[tenant()]",
  subscription: subscription()
//@      "subscription": "[subscription()]"
}

