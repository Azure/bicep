// Availability Set
resource availabilitySet 'Microsoft.Compute/availabilitySets@2020-12-01' = {
  name: ${1:'availabilitySet'}
  location: resourceGroup().location
}
