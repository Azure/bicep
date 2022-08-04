// $1 = workSpace
// $2 = 'name'
// $3 = location
// $4 = 'friendlyName'

param location string

resource workSpace 'Microsoft.DesktopVirtualization/workspaces@2021-07-12' = {
  name: 'name'
  location: location
  properties: {
    friendlyName: 'friendlyName'
  }
}
// Insert snippet here

