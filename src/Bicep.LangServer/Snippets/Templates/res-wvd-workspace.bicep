// WVD Workspace
resource ${1:'workSpace'} 'Microsoft.DesktopVirtualization/workspaces@2019-12-10-preview' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    friendlyName: ${2:'friendlyName'}
  }
}
