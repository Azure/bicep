resource sqlServer 'Microsoft.Sql/servers@2020-11-01-preview' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    administratorLogin: 'administratorLogin'
    administratorLoginPassword: 'administratorLoginPassword'
  }
}

resource sqlServerFirewallRules 'Microsoft.Sql/servers/firewallRules@2020-11-01-preview' = {
  parent: sqlServer
  name: 'name'
  properties: {
    startIpAddress: 'startIpAddress'
    endIpAddress: 'endIpAddress'
  }
}

