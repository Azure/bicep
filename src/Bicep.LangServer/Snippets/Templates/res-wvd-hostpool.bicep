// WVD Host Pool
resource ${1:'hostPool'} 'Microsoft.DesktopVirtualization/hostpools@2019-12-10-preview' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    friendlyName: ${2:'hostpoolFriendlyName'}
    hostPoolType: '${3|Personal,Pooled|}'
    loadBalancerType: '${4|BreadthFirst,DepthFirst,Persistent|}'
    preferredAppGroupType: '${5|Desktop,RailApplications,None|}'
  }
}
