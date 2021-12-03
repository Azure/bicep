// SQL Server Firewall Rules
resource sqlServer 'Microsoft.Sql/servers@2020-11-01-preview' = {
  name: /*${1:'name'}*/'name'
  location: location
  properties: {
    administratorLogin: /*${2:'administratorLogin'}*/'administratorLogin'
    administratorLoginPassword: /*${3:'administratorLoginPassword'}*/'administratorLoginPassword'
  }
}

resource /*${4:sqlServerFirewallRules}*/sqlServerFirewallRules 'Microsoft.Sql/servers/firewallRules@2020-11-01-preview' = {
  parent: sqlServer
  name: /*${5:'name'}*/'name'
  properties: {
    startIpAddress: /*${6:'startIpAddress'}*/'startIpAddress'
    endIpAddress: /*${7:'endIpAddress'}*/'endIpAddress'
  }
}
