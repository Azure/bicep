targetScope='managementGroup'

var deploymentLocation = deployment().location
//@[4:22) Variable deploymentLocation. Type: string. Declaration start char: 0, length: 46

var scopesWithArmRepresentation = {
//@[4:31) Variable scopesWithArmRepresentation. Type: object. Declaration start char: 0, length: 93
  tenant: tenant()
  managementGroup: managementGroup()
}

