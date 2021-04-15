resource ws 'Microsoft.DesktopVirtualization/workspaces@2019-12-10-preview' = {
  name: 'testWorkSpace'
  location: resourceGroup().location
  properties: {
    friendlyName: 'testFriendlyName'
  }
}

