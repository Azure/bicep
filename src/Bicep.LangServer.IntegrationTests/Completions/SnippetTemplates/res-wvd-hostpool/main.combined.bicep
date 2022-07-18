// $1 = hostPool
// $2 = 'name'
// $3 = location
// $4 = 'friendlyName'
// $5 = 'Pooled'
// $6 = 'BreadthFirst'
// $7 = 'Desktop'

param location string

resource hostPool 'Microsoft.DesktopVirtualization/hostpools@2021-07-12' = {
  name: 'name'
  location: location
  properties: {
    friendlyName: 'friendlyName'
    hostPoolType: 'Pooled'
    loadBalancerType: 'BreadthFirst'
    preferredAppGroupType: 'Desktop'
  }
}
// Insert snippet here

