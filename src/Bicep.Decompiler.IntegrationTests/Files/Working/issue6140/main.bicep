resource someworkspacename 'microsoft.operationalinsights/workspaces@2021-06-01' = {
  name: 'someworkspacename'
  location: 'Some Location'
//@[12:27) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'Some Location' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'Some Location'|
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
