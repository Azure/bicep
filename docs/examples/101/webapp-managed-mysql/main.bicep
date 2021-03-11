param websiteName string
param dbAdminLogin string

@secure()
@minLength(8)
@maxLength(128)
param dbAdminPassword string

@allowed([
  2
  4
  8
  16
  32
])
param dbSkuCapacity int = 2

@allowed([
  'GP_Gen5_2'
  'GP_Gen5_4'
  'GP_Gen5_8'
  'GP_Gen5_16'
  'GP_Gen5_32'
  'MO_Gen5_2'
  'MO_Gen5_4'
  'MO_Gen5_8'
  'MO_Gen5_16'
  'MO_Gen5_32'
])
param dbSkuName string = 'GP_Gen5_2'

@allowed([
  51200
  102400
])
param dbSkuSizeInMB int = 51200

@allowed([
  'GeneralPurpose'
  'MemoryOptimized'
])
param dbSkuTier string = 'GeneralPurpose'

param dbSkuFamily string = 'Gen5'

@allowed([
  '5.6'
  '5.7'
])
param mySQLVersion string

param location string = resourceGroup().location

var dbName = '${websiteName}-db'
var dbServerName = '${websiteName}-server'
var serverFarmName = '${websiteName}-serviceplan'

resource serverFarm 'Microsoft.Web/serverfarms@2020-06-01' = {
  name: serverFarmName
  location: location
  sku: {
    tier: 'Standard'
    name: 'S1'
  }
}
resource website 'Microsoft.Web/sites@2020-06-01' = {
  name: websiteName
  location: location
  properties: {
    serverFarmId: serverFarm.id
  }
}
resource connectionString 'Microsoft.Web/sites/config@2020-06-01' = {
  name: '${website.name}/connectionString'
  properties: {
    defaultConnection: {
      value: 'Database=${dbName};Data Source=${dbServer.properties.fullyQualifiedDomainName};User Id=${dbAdminLogin}@${dbServer.name};Password=${dbAdminPassword}'
      type: 'MySql'
    }
  }
}
resource dbServer 'Microsoft.DBForMySQL/servers@2017-12-01-preview' = {
  name: dbServerName
  location: location
  sku: {
    name: dbSkuName
    tier: dbSkuTier
    capacity: dbSkuCapacity
    size: string(dbSkuSizeInMB)
    family: dbSkuFamily
  }
  properties: {
    createMode: 'Default'
    version: mySQLVersion
    administratorLogin: dbAdminLogin
    administratorLoginPassword: dbAdminPassword
    storageProfile: {
      storageMB: dbSkuSizeInMB
      backupRetentionDays: 7
      geoRedundantBackup: 'Disabled'
    }
    sslEnforcement: 'Disabled'
  }
}
resource firewallRules 'Microsoft.DBForMySQL/servers/firewallRules@2017-12-01-preview' = {
  name: '${dbServer.name}/allowAzureIPs'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}
resource database 'Microsoft.DBForMySQL/servers/databases@2017-12-01-preview' = {
  name: '${dbServer.name}/${dbName}'
  properties: {
    charset: 'utf8'
    collation: 'utf8_general_ci'
  }
}
