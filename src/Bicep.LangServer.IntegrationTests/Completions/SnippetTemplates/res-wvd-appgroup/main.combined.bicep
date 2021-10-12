// $1 = applicationGroup
// $2 = 'name'
// $3 = 'friendlyName'
// $4 = 'Desktop'
// $5 = 'desktopVirtualizationHostPools.id'

resource applicationGroup 'Microsoft.DesktopVirtualization/applicationgroups@2021-07-12' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    friendlyName: 'friendlyName'
    applicationGroupType: 'Desktop'
    hostPoolArmPath: 'desktopVirtualizationHostPools.id'
  }
}
// Insert snippet here

