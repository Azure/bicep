resource ag 'Microsoft.DesktopVirtualization/applicationgroups@2019-12-10-preview' = {
  name: 'testAppGroup'
  location: resourceGroup().location
  properties: {
    friendlyName: 'testFriendlyName'
    applicationGroupType: 'Desktop'
    hostPoolArmPath: resourceId('Microsoft.DesktopVirtualization/hostpools', 'testHostPool')
  }
}

