// SQL Server Firewall Rules
resource sqlServer 'Microsoft.Sql/servers@2020-11-01-preview' = {
  name: ${1:'name'}
  location: resourceGroup().location
  properties: {
    administratorLogin: ${2:'administratorLogin'}
    administratorLoginPassword: ${3:'administratorLoginPassword'}
  }
}

resource ${4:sqlServerFirewallRules} 'Microsoft.Sql/servers/firewallRules@2020-11-01-preview' = {
  parent: sqlServer
  name: ${5:'name'}
  properties: {
    startIpAddress: ${6:'startIpAddress'}
    endIpAddress: ${7:'endIpAddress'}
  }
}
