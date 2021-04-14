// WVD Host Pool
resource hp 'Microsoft.DesktopVirtualization/hostpools@2019-12-10-preview' = {
  name: ${1:hostpoolName}
  location: resourceGroup().location
  properties: {
    friendlyName: ${2:hostpoolFriendlyName}
    hostPoolType: ${3|Personal,Pooled|}
    loadBalancerType: ${4|BreadthFirst,DepthFirst,Persistent|}
    preferredAppGroupType: ${5|Desktop,RailApplications,None|}
  }
}
