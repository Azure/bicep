//Define WVD deployment parameters
param hostpoolName string
param hostpoolFriendlyName string
param appgroupName string
param appgroupNameFriendlyName string
param workspaceName string
param workspaceNameFriendlyName string
param applicationgrouptype string = 'Desktop'
param preferredAppGroupType string = 'Desktop'
param wvdbackplanelocation string = 'eastus'
param hostPoolType string = 'pooled'
param loadBalancerType string = 'BreadthFirst'
param logAnalyticsWorkspaceName string
param logAnalyticslocation string = 'westeurope'
param logAnalyticsWorkspaceSku string = 'pergb2018'
param logAnalyticsResourceGroup string
param wvdBackplaneResourceGroup string

//Create WVD Hostpool
resource hp 'Microsoft.DesktopVirtualization/hostpools@2019-12-10-preview' = {
  name: hostpoolName
  location: wvdbackplanelocation
  properties: {
    friendlyName: hostpoolFriendlyName
    hostPoolType: hostPoolType
    loadBalancerType: loadBalancerType
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

//Create Azure Log Analytics Workspace
module wvdmonitor './wvd-LogAnalytics.bicep' = {
  name: 'LAWorkspace'
  scope: resourceGroup(logAnalyticsResourceGroup)
  params: {
    logAnalyticsWorkspaceName: logAnalyticsWorkspaceName
    logAnalyticslocation: logAnalyticslocation
    logAnalyticsWorkspaceSku: logAnalyticsWorkspaceSku
    hostpoolName: hp.name
    workspaceName: ws.name
    logAnalyticsResourceGroup: logAnalyticsResourceGroup
    wvdBackplaneResourceGroup: wvdBackplaneResourceGroup
  }
}
