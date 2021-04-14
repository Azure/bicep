resource ws 'Microsoft.DesktopVirtualization/workspaces@2019-12-10-preview' = {
  name: 'testWorkSpace'
  location: wresourceGroup().location
  properties: {
    friendlyName: 'testFriendlyName'
  }
}

