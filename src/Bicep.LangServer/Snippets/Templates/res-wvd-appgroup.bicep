// WVD AppGroup
resource applicationGroup 'Microsoft.DesktopVirtualization/applicationgroups@2019-12-10-preview' = {
  name: ${1:'applicationGroup'}
  location: resourceGroup().location
  properties: {
    friendlyName: ${2:'friendlyName'}
    applicationGroupType: '${3|Desktop,RailApplications|}'
    hostPoolArmPath: resourceId('Microsoft.DesktopVirtualization/hostpools', ${4:'hostpoolName'})
  }
}
