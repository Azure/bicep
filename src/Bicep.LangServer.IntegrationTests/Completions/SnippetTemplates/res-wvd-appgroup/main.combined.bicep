resource applicationGroup 'Microsoft.DesktopVirtualization/applicationgroups@2019-12-10-preview' = {
  name: 'applicationGroup'
  location: resourceGroup().location
  properties: {
    friendlyName: 'friendlyName'
    applicationGroupType: 'Desktop'
    hostPoolArmPath: resourceId('Microsoft.DesktopVirtualization/hostpools', 'hostPool')
  }
}

