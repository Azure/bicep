// $1 = availabilitySet
// $2 = 'name'

resource availabilitySet 'Microsoft.Compute/availabilitySets@2020-12-01' = {
  name: 'name'
  location: resourceGroup().location
}
// Insert snippet here

