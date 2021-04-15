resource hp 'Microsoft.DesktopVirtualization/hostpools@2019-12-10-preview' = {
  name: 'testHostPool'
  location: resourceGroup().location
  properties: {
    friendlyName: 'testFriendlyName'
    hostPoolType: 'Pooled'
    loadBalancerType: 'BreadthFirst'
    preferredAppGroupType: 'Desktop'
  }
}

