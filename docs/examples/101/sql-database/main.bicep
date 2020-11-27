param serverName string {
  default: uniqueString('sql', resourceGroup().id)
}
param sqlDBName string {
  default: 'SampleDB'
}
param location string {
  default: resourceGroup().location
}
param administratorLogin string {}
param administratorLoginPassword string {
  secure: true
}

resource server 'Microsoft.Sql/servers@2019-06-01-preview' = {
  name: serverName
  location: location
  properties: {
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorLoginPassword
  }
}

resource sqlDB 'Microsoft.Sql/servers/databases@2020-08-01-preview' = {
  name: '${server.name}/${sqlDBName}'
  location: location
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
}
