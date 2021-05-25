// WVD AppGroup
resource ${1:applicationGroup} 'Microsoft.DesktopVirtualization/applicationgroups@2019-12-10-preview' = {
  name: ${2:'name'}
  location: resourceGroup().location
  properties: {
    friendlyName: ${3:'friendlyName'}
    applicationGroupType: ${4|'Desktop','RemoteApp'|}
    hostPoolArmPath: resourceId('Microsoft.DesktopVirtualization/hostpools', ${5:'REQUIRED'})
  }
}
