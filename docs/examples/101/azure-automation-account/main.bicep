param sku string = 'pergb2018'
param dataRetention int = 30
param location string = resourceGroup().location
param appName string = uniqueString(resourceGroup().id)

var workspaceName = toLower('la-${appName}')
var automationaccountName = toLower('aa${appName}')
var automationaccountDiagName = toLower('diag-aa${appName}')

resource automation_log_analytics 'Microsoft.OperationalInsights/workspaces@2020-08-01' = {
  location: location
  name: workspaceName
  properties: {
    sku: {
      name: sku
    }
    retentionInDays: dataRetention
  }
}

resource automation_account 'Microsoft.Automation/automationAccounts@2020-01-13-preview' = {
  location: location
  name: automationaccountName
  properties: {
    sku: {
      name: 'Basic'
    }
  }
  identity: {
    type: 'SystemAssigned'
  }
}

resource automation_account_diagnostic 'microsoft.insights/diagnosticSettings@2017-05-01-preview' = {
  name: automationaccountDiagName
  scope: automation_account
  properties: {
    workspaceId: automation_log_analytics.id
    logs: [
      {
        category: 'JobLogs'
        enabled: true
      }
    ]
  }
}
