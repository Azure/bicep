// $1 = availabilitySet
// $2 = 'name'
// $3 = location

param location string

resource availabilitySet 'Microsoft.Compute/availabilitySets@2020-12-01' = {
  name: 'name'
  location: location
}
// Insert snippet here

