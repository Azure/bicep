// $1 = 'name'
// $2 = location
// $3 = 'administratorLogin'
// $4 = 'administratorLoginPassword'
// $5 = sqlServerFirewallRules
// $6 = 'name'
// $7 = 'startIpAddress'
// $8 = 'endIpAddress'

param location string

resource sqlServer 'Microsoft.Sql/servers@2021-02-01-preview' = {
  name: 'name'
  location: location
  properties: {
    administratorLogin: 'administratorLogin'
    administratorLoginPassword: 'administratorLoginPassword'
  }
}

resource sqlServerFirewallRules 'Microsoft.Sql/servers/firewallRules@2021-02-01-preview' = {
  parent: sqlServer
  name: 'name'
  properties: {
    startIpAddress: 'startIpAddress'
    endIpAddress: 'endIpAddress'
  }
}
// Insert snippet here

