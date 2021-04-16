// WVD AppGroup
resource ag 'Microsoft.DesktopVirtualization/applicationgroups@2019-12-10-preview' = {
  name: ${1:appGroupName}
  location: resourceGroup().location
  properties: {
    friendlyName: ${2:appgroupNameFriendlyName}
    applicationGroupType: ${3|Desktop,RailApplications|}
    hostPoolArmPath: resourceId('Microsoft.DesktopVirtualization/hostpools', ${4:hostpoolName})
  }
}
