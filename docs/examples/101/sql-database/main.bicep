param serverName string {
  metadata: {
    description: 'The name of the SQL logical server.'
  }
  default: uniqueString('sql', resourceGroup().id)
}
param sqlDBName string {
  metadata: {
    description: 'The name of the SQL Database.'
  }
  default: 'SampleDB'
}
param location string {
  metadata: {
    description: 'Location for all resources.'
  }
  default: resourceGroup().location
}
param administratorLogin string {
  metadata: {
    description: 'The administrator username of the SQL logical server.'
  }
}
param administratorLoginPassword string {
  metadata: {
    description: 'The administrator password of the SQL logical server.'
  }
  secure: true
}

resource serverName_resource 'Microsoft.Sql/servers@2019-06-01-preview' = {
  name: serverName
  location: location
  properties: {
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorLoginPassword
  }
}

resource serverName_sqlDBName 'Microsoft.Sql/servers/databases@2020-08-01-preview' = {
  name: '${serverName_resource.name}/${sqlDBName}'
  location: location
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
}