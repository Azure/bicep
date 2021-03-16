resource availabilitySet1 'Microsoft.Compute/availabilitySets@2019-07-01' = {
  name: '${1:availabilitySet1}'
  location: resourceGroup().location
  tags: {
    displayName: '${1:availabilitySet1}'
  }
  properties: {}
}