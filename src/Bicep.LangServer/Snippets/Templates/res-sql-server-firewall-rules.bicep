// SQL Server Firewall Rules
resource sqlServer 'Microsoft.Sql/servers@2021-02-01-preview' = {
  name: /*${1:'name'}*/'name'
  location: /*${2:location}*/'location'
  properties: {
    administratorLogin: /*${3:'administratorLogin'}*/'administratorLogin'
    administratorLoginPassword: /*${4:'administratorLoginPassword'}*/'administratorLoginPassword'
  }
}

resource /*${5:sqlServerFirewallRules}*/sqlServerFirewallRules 'Microsoft.Sql/servers/firewallRules@2021-02-01-preview' = {
  parent: sqlServer
  name: /*${6:'name'}*/'name'
  properties: {
    startIpAddress: /*${7:'startIpAddress'}*/'startIpAddress'
    endIpAddress: /*${8:'endIpAddress'}*/'endIpAddress'
  }
}
