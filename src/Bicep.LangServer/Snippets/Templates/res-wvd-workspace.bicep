// WVD Workspace
resource /*${1:workSpace}*/workSpace 'Microsoft.DesktopVirtualization/workspaces@2019-12-10-preview' = {
  name: /*${2:'name'}*/'name'
  location: location
  properties: {
    friendlyName: /*${3:'friendlyName'}*/'friendlyName'
  }
}
