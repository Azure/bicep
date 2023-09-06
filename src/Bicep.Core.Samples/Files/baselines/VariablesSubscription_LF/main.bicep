targetScope='subscription'

var deploymentLocation = deployment().location

var scopesWithArmRepresentation = {
  tenant: tenant()
  subscription: subscription()
}
