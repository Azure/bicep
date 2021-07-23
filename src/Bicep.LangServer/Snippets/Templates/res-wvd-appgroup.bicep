// WVD AppGroup
resource /*${1:desktopVirtualizationHostPools}*/desktopVirtualizationHostPools 'Microsoft.DesktopVirtualization/hostPools@2021-05-13-preview' existing = {
  name: /*${2:'name'}*/'name'
} 

resource /*${3:applicationGroup}*/applicationGroup 'Microsoft.DesktopVirtualization/applicationgroups@2019-12-10-preview' = {
  name: /*${4:'name'}*/'name'
  location: resourceGroup().location
  properties: {
    friendlyName: /*${5:'friendlyName'}*/'friendlyName'
    applicationGroupType: /*${6|'Desktop','RemoteApp'|}*/'Desktop'
    hostPoolArmPath: desktopVirtualizationHostPools.id
  }
}
