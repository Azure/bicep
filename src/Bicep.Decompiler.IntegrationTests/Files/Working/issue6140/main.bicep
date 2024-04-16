resource someworkspacename 'microsoft.operationalinsights/workspaces@2021-06-01' = {
  name: 'someworkspacename'
  location: 'Some Location'
  properties: {
    provisioningState: 'Succeeded'
    sku: {
      name: 'pergb2018'
    }
    retentionInDays: 365
    features: {
      enableLogAccessUsingOnlyResourcePermissions: true
    }
    workspaceCapping: {
      dailyQuotaGb: -1
    }
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

