resource workSpace 'Microsoft.DesktopVirtualization/workspaces@2019-12-10-preview' = {
  name: 'workSpace'
  location: resourceGroup().location
  properties: {
    friendlyName: 'friendlyName'
  }
}

