// Availability Set
resource ${1:'availabilitySet'} 'Microsoft.Compute/availabilitySets@2020-12-01' = {
  name: 'name'
  location: resourceGroup().location
}
