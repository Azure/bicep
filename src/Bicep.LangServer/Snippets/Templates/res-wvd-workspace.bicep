// WVD Workspace
resource ${1:workSpace} 'Microsoft.DesktopVirtualization/workspaces@2019-12-10-preview' = {
  name: ${2:'name'}
  location: resourceGroup().location
  properties: {
    friendlyName: ${3:'friendlyName'}
  }
}
