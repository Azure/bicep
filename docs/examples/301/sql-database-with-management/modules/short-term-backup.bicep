param sqlDatabase object
param sqlServerName string

// Short term backup
resource shortTermBackup 'Microsoft.Sql/servers/databases/backupShortTermRetentionPolicies@2020-08-01-preview' = {
  name: '${sqlServerName}/${sqlDatabase.name}/Default'
  properties: {
    retentionDays: sqlDatabase.shortTermBackupRetention
  }
}