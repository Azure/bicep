// $1 = applicationGroup
// $2 = 'name'
// $3 = location
// $4 = 'friendlyName'
// $5 = 'Desktop'
// $6 = 'desktopVirtualizationHostPools.id'

param location string

resource applicationGroup 'Microsoft.DesktopVirtualization/applicationgroups@2021-07-12' = {
  name: 'name'
  location: location
  properties: {
    friendlyName: 'friendlyName'
    applicationGroupType: 'Desktop'
    hostPoolArmPath: 'desktopVirtualizationHostPools.id'
  }
}
// Insert snippet here

