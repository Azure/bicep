// WVD Workspace
resource /*${1:workSpace}*/workSpace 'Microsoft.DesktopVirtualization/workspaces@2021-07-12' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: {
    friendlyName: /*${4:'friendlyName'}*/'friendlyName'
  }
}
