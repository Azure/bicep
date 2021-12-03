// $1 = applicationGroup
// $2 = 'name'
// $3 = 'friendlyName'
// $4 = 'Desktop'
// $5 = 'desktopVirtualizationHostPools.id'

param location string

resource applicationGroup 'Microsoft.DesktopVirtualization/applicationgroups@2019-12-10-preview' = {
  name: 'name'
  location: location
  properties: {
    friendlyName: 'friendlyName'
    applicationGroupType: 'Desktop'
    hostPoolArmPath: 'desktopVirtualizationHostPools.id'
  }
}
// Insert snippet here

