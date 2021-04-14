// WVD Workspace
resource ws 'Microsoft.DesktopVirtualization/workspaces@2019-12-10-preview' = {
  name: ${1:workspaceName}
  location: wresourceGroup().location
  properties: {
    friendlyName: ${2:workspaceFriendlyName}
  }
}
