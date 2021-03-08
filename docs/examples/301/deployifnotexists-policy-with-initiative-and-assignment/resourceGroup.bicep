// DEPLOYMENT SCOPE
targetScope = 'subscription'

// PARAMETERS
param resourceGroupName string
param location string

// VARIABLES

// OUTPUTS
output rgId string = rg.id
output rgName string = rg.name

// RESOURCES
resource rg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
  name: resourceGroupName
  location: location
}