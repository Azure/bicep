using 'main.bicep'
import * as bicepconfig from 'bicepconfig.bicep'
// ok
param one = bicepconfig.directExport
// Failed to evaluate parameter "two"
// Unhandled exception during evaluating template language function 'variables' is not handled.bicep(BCP338)
param two = bicepconfig.functionExport

