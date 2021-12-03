// WVD AppGroup
resource /*${1:applicationGroup}*/applicationGroup 'Microsoft.DesktopVirtualization/applicationgroups@2019-12-10-preview' = {
  name: /*${2:'name'}*/'name'
  location: location
  properties: {
    friendlyName: /*${3:'friendlyName'}*/'friendlyName'
    applicationGroupType: /*${4|'Desktop','RemoteApp'|}*/'Desktop'
    hostPoolArmPath: /*${5:'desktopVirtualizationHostPools.id'}*/'desktopVirtualizationHostPools.id'
  }
}
