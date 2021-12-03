// Availability Set
resource /*${1:availabilitySet}*/availabilitySet 'Microsoft.Compute/availabilitySets@2020-12-01' = {
  name: /*${2:'name'}*/'name'
  location: location
}
