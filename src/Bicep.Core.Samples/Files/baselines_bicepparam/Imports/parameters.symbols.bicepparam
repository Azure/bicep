using 'main.bicep'
import * as bicepconfig from 'bicepconfig.bicep'
// ok
param one = bicepconfig.directExport
//@[6:9) ParameterAssignment one. Type: bool. Declaration start char: 0, length: 36
// Failed to evaluate parameter "two"
// Unhandled exception during evaluating template language function 'variables' is not handled.bicep(BCP338)
param two = bicepconfig.functionExport
//@[6:9) ParameterAssignment two. Type: bool. Declaration start char: 0, length: 38

