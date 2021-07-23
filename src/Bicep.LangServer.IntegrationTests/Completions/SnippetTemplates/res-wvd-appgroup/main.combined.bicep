// $1 = desktopVirtualizationHostPools
// $2 = 'name'
// $3 = applicationGroup
// $4 = 'name'
// $5 = 'friendlyName'
// $6 = 'Desktop'

resource desktopVirtualizationHostPools 'Microsoft.DesktopVirtualization/hostPools@2021-05-13-preview' existing = {
  name: 'name'
} 

resource applicationGroup 'Microsoft.DesktopVirtualization/applicationgroups@2019-12-10-preview' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    friendlyName: 'friendlyName'
    applicationGroupType: 'Desktop'
    hostPoolArmPath: desktopVirtualizationHostPools.id
  }
}
// Insert snippet here
