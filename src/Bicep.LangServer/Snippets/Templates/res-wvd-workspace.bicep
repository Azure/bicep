﻿// WVD Workspace
resource /*${1:workSpace}*/workSpace 'Microsoft.DesktopVirtualization/workspaces@2019-12-10-preview' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: {
    friendlyName: /*${4:'friendlyName'}*/'friendlyName'
  }
}
