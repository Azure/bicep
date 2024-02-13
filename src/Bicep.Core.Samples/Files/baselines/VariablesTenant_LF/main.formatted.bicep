targetScope = 'tenant'

var deploymentLocation = deployment().location

var scopesWithArmRepresentation = {
  tenant: tenant()
}
