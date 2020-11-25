//Define Log Analytics parameters
param logAnalyticsWorkspaceName string
param logAnalyticslocation string = 'westeurope'
param logAnalyticsWorkspaceSku string = 'pergb2018'
param hostpoolName string
param workspaceName string
param logAnalyticsResourceGroup string
param wvdBackplaneResourceGroup string

//Creaye Log Analytics Workspace
resource wvdla 'Microsoft.OperationalInsights/workspaces@2020-08-01' = {
  name: logAnalyticsWorkspaceName
  location: logAnalyticslocation
  properties: {
    sku: {
      name: logAnalyticsWorkspaceSku
    }
  }
}

//Create Diagnotic Setting for WVD components
module wvdmonitor './wvd-monitor-diag.bicep' = {
  name: 'myBicepLADiag'
  scope: resourceGroup(wvdBackplaneResourceGroup)
  params: {
    logAnalyticslocation: logAnalyticslocation
    logAnalyticsWorkspaceID: wvdla.id
    hostpoolName: hostpoolName
    workspaceName: workspaceName
  }
}