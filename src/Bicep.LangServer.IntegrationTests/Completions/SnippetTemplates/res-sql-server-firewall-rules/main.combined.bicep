// $1 = 'name'
// $2 = 'administratorLogin'
// $3 = 'administratorLoginPassword'
// $4 = sqlServerFirewallRules
// $5 = 'name'
// $6 = 'startIpAddress'
// $7 = 'endIpAddress'

param location string

resource sqlServer 'Microsoft.Sql/servers@2020-11-01-preview' = {
  name: 'name'
  location: location
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
// Insert snippet here

