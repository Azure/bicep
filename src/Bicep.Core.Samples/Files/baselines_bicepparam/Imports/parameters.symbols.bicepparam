using 'main.bicep'
import * as bicepconfig from 'bicepconfig.bicep'
//@[12:23) ImportedNamespace bicepconfig. Type: bicepconfig. Declaration start char: 7, length: 16
// ok
param one = bicepconfig.directExport
//@[06:09) ParameterAssignment one. Type: true. Declaration start char: 0, length: 36
// Failed to evaluate parameter "two"
// Unhandled exception during evaluating template language function 'variables' is not handled.bicep(BCP338)
param two = bicepconfig.functionExport
//@[06:09) ParameterAssignment two. Type: bool. Declaration start char: 0, length: 38

