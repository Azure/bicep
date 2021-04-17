// WVD Workspace
resource workSpace 'Microsoft.DesktopVirtualization/workspaces@2019-12-10-preview' = {
  name: ${1:'workSpace'}
  location: resourceGroup().location
  properties: {
    friendlyName: ${2:'friendlyName'}
  }
}
