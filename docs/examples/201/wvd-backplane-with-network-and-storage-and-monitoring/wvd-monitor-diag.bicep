//Define diagnostic setting  parameters
param logAnalyticsWorkspaceID string
param logAnalyticslocation string = 'westeurope'
param hostpoolName string
param workspaceName string

//Concat diagnostic setting names
var hostpoolDiagName = '${hostpoolName}/Microsoft.Insights/hostpool-diag'
var workspaceDiagName = '${workspaceName}/Microsoft.Insights/workspacepool-diag'

//Create diagnostic settings for WVD Objects
resource wvdhpds 'Microsoft.DesktopVirtualization/hostpools/providers/diagnosticSettings@2017-05-01-preview' = {
  name: hostpoolDiagName
  location: logAnalyticslocation
  properties: {
    workspaceId: logAnalyticsWorkspaceID
    logs: [
      {
        category: 'Checkpoint'
        enabled: 'True'
      }
      {
        category: 'Error'
        enabled: 'True'
      }
      {
        category: 'Management'
        enabled: 'True'
      }
      {
        category: 'Connection'
        enabled: 'True'
      }
      {
        category: 'HostRegistration'
        enabled: 'True'
      }
    ]
  }
}

resource wvdwsds 'Microsoft.DesktopVirtualization/workspaces/providers/diagnosticSettings@2017-05-01-preview' = {
  name: workspaceDiagName
  location: logAnalyticslocation
  properties: {
    workspaceId: logAnalyticsWorkspaceID
    logs: [
      {
        category: 'Checkpoint'
        enabled: 'True'
      }
      {
        category: 'Error'
        enabled: 'True'
      }
      {
        category: 'Management'
        enabled: 'True'
      }
      {
        category: 'Feed'
        enabled: 'True'
      }
    ]
  }
}