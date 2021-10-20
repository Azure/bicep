// $1 = workSpace
// $2 = 'name'
// $3 = 'friendlyName'

param location string

resource workSpace 'Microsoft.DesktopVirtualization/workspaces@2019-12-10-preview' = {
  name: 'name'
  location: location
  properties: {
    friendlyName: 'friendlyName'
  }
}
// Insert snippet here

