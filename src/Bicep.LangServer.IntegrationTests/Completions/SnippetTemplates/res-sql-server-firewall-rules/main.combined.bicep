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

resource 'administratorLoginPassword' 'Microsoft.Sql/servers/firewallRules@2021-02-01-preview' = {
//@[09:37) [BCP017 (Error)] Expected a resource identifier at this location. (CodeDescription: none) |'administratorLoginPassword'|
//@[09:37) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |'administratorLoginPassword'|
//@[38:94) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) |'Microsoft.Sql/servers/firewallRules@2021-02-01-preview'|
  parent: sqlServer
  name: 'name'
  properties: {
    startIpAddress: 'startIpAddress'
    endIpAddress: 'endIpAddress'
  }
}
// Insert snippet here

