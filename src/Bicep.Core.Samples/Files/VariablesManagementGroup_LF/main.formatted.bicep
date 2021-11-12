targetScope = 'managementGroup'

var deploymentLocation = deployment().location

var scopesWithArmRepresentation = {
  tenant: tenant()
  managementGroup: managementGroup()
}
