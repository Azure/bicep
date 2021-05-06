//Define diagnostic setting  parameters
param logAnalyticsWorkspaceID string
param hostpoolName string
param workspaceName string

resource hostPool 'Microsoft.DesktopVirtualization/hostPools@2020-11-02-preview' existing = {
  name: hostpoolName
}

resource workspace 'Microsoft.DesktopVirtualization/workspaces@2020-11-02-preview' existing = {
  name: workspaceName
}

//Create diagnostic settings for WVD Objects
resource wvdhpds 'Microsoft.Insights/diagnosticSettings@2017-05-01-preview' = {
  scope: hostPool

  name: 'hostpool-diag'
  properties: {
    workspaceId: logAnalyticsWorkspaceID
    logs: [
      {
        category: 'Checkpoint'
        enabled: true
      }
      {
        category: 'Error'
        enabled: true
      }
      {
        category: 'Management'
        enabled: true
      }
      {
        category: 'Connection'
        enabled: true
      }
      {
        category: 'HostRegistration'
        enabled: true
      }
    ]
  }
}

resource wvdwsds 'Microsoft.Insights/diagnosticSettings@2017-05-01-preview' = {
  scope: hostPool

  name: 'workspacepool-diag'
  properties: {
    workspaceId: logAnalyticsWorkspaceID
    logs: [
      {
        category: 'Checkpoint'
        enabled: true
      }
      {
        category: 'Error'
        enabled: true
      }
      {
        category: 'Management'
        enabled: true
      }
      {
        category: 'Feed'
        enabled: true
      }
    ]
  }
}
