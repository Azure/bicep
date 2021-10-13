// WVD Workspace
resource /*${1:workSpace}*/workSpace 'Microsoft.DesktopVirtualization/workspaces@2021-07-12' = {
  name: /*${2:'name'}*/'name'
  location: resourceGroup().location
  properties: {
    friendlyName: /*${3:'friendlyName'}*/'friendlyName'
  }
}
