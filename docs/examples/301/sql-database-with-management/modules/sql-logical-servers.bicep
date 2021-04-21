@description('SQL logical servers.')
param sqlLogicalServers array
param tags object

@secure()
param password string

var defaultSqlLogicalServerProperties = {
  name: ''
  tags: {}
  userName: ''
  passwordFromKeyVault: {
    subscriptionId: subscription().subscriptionId
    resourceGroupName: ''
    name: ''
    secretName: ''
  }
  systemManagedIdentity: false
  minimalTlsVersion: '1.2'
  publicNetworkAccess: 'Enabled'
  azureActiveDirectoryAdministrator: {
    name: ''
    objectId: ''
    tenantId: subscription().tenantId
  }
  firewallRules: []
  azureDefender: {
    enabled: false
    emailAccountAdmins: false
    emailAddresses: []
    disabledRules: []
    vulnerabilityAssessments: {
      recurringScans: false
      storageAccount: {
        resourceGroupName: ''
        name: ''
        containerName: ''
      }
      emailSubscriptionAdmins: false
      emails: []
    }
  }
  auditActionsAndGroups: []
  diagnosticLogsAndMetrics: {
    name: ''
    resourceGroupName: ''
    subscriptionId: subscription().subscriptionId
    logs: []
    metrics: []
    auditLogs: false
    microsoftSupportOperationsAuditLogs: false
  }
  databases: []
}

module sqlLogicalServer 'sql-logical-server.bicep' = [for (sqlLogicalServer, index) in sqlLogicalServers: {
  name: 'sqlLogicalServer-${index}'
  params: {
    sqlLogicalServer: union(defaultSqlLogicalServerProperties, sqlLogicalServer)
    password: password
    tags: union(tags, union(defaultSqlLogicalServerProperties, sqlLogicalServer).tags)
  }
}]