resource hostPool 'Microsoft.DesktopVirtualization/hostpools@2019-12-10-preview' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    friendlyName: 'friendlyName'
    hostPoolType: 'Pooled'
    loadBalancerType: 'BreadthFirst'
    preferredAppGroupType: 'Desktop'
  }
}

