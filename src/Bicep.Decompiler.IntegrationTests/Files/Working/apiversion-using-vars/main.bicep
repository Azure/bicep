var apiVersions = {
//@[4:15) [no-unused-vars (Warning)] Variable "apiVersions" is declared but never used. (bicep core linter https://aka.ms/bicep/linter/no-unused-vars) |apiVersions|
  nsg: '2019-11-01'
}
var nsgApiVersion = '2019-11-01'
//@[4:17) [no-unused-vars (Warning)] Variable "nsgApiVersion" is declared but never used. (bicep core linter https://aka.ms/bicep/linter/no-unused-vars) |nsgApiVersion|

resource foo 'Microsoft.Network/networkSecurityGroups@2019-11-01' = {
  name: 'foo'
  location: 'West US'
  properties: {}
}

resource foo2 'Microsoft.Network/networkSecurityGroups@2019-11-01' = {
  name: 'foo2'
  location: 'West US'
  properties: {}
}

