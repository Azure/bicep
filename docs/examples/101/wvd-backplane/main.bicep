//Define WVD deployment parameters
param hostpoolName string = 'myFirstHostpool'
param hostpoolFriendlyName string = 'My Bicep created Host pool'
param appgroupName string = 'myFirstAppGroup'
param appgroupNameFriendlyName string = 'My Bicep created AppGroup'
param workspaceName string = 'myFirstWortkspace'
param workspaceNameFriendlyName string = 'My Bicep created Workspace'
param applicationgrouptype string = 'Desktop'
param preferredAppGroupType string = 'Desktop'
param wvdbackplanelocation string = 'eastus'
param hostPoolType string = 'pooled'
param loadBalancerType string = 'BreadthFirst'

//Create WVD Hostpool
resource hp 'Microsoft.DesktopVirtualization/hostpools@2019-12-10-preview' = {
  name: hostpoolName
  location: wvdbackplanelocation
  properties: {
    friendlyName: hostpoolFriendlyName
    hostPoolType : hostPoolType
    loadBalancerType : loadBalancerType
    preferredAppGroupType: preferredAppGroupType
    
  }
}

//Create WVD AppGroup
resource ag 'Microsoft.DesktopVirtualization/applicationgroups@2019-12-10-preview' = {
  name: appgroupName
  location: wvdbackplanelocation
  properties: {
      friendlyName: appgroupNameFriendlyName
      applicationGroupType: applicationgrouptype
      hostPoolArmPath: hp.id
    }
  }

//Create WVD Workspace
resource ws 'Microsoft.DesktopVirtualization/workspaces@2019-12-10-preview' = {
  name: workspaceName
  location: wvdbackplanelocation
  properties: {
      friendlyName: workspaceNameFriendlyName
      applicationGroupReferences: [
        ag.id
      ]
  }
}
  
