// WVD Host Pool
resource ${1:'hostPool'} 'Microsoft.DesktopVirtualization/hostpools@2019-12-10-preview' = {
  name: ${2:'name'}
  location: resourceGroup().location
  properties: {
    friendlyName: ${3:'hostpoolFriendlyName'}
    hostPoolType: '${4|Personal,Pooled|}'
    loadBalancerType: '${5|BreadthFirst,DepthFirst,Persistent|}'
    preferredAppGroupType: '${6|Desktop,RailApplications,None|}'
  }
}
