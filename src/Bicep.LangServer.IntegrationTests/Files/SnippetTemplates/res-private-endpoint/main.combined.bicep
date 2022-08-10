// $1 = privateEndpoint
// $2 = 'name'
// $3 = 'location'
// $4 = 'name'
// $5 = 'privateLinkServiceId'
// $6 = 'groupId'
// $7 = 'subnetId'

param location string
//@[06:14) [no-unused-params (Warning)] Parameter "location" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |location|

resource privateEndpoint 'Microsoft.Network/privateEndpoints@2022-01-01' = {
  name: 'name'
  location: 'location'
//@[12:22) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'location' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'location'|
  properties: {
    privateLinkServiceConnections: [
      {
        name: 'name'
        properties: {
          privateLinkServiceId: 'privateLinkServiceId'
          groupIds: [
            'groupId'
          ]
        }
      }
    ]
    subnet: {
      id: 'subnetId'
    }
  }
}
// Insert snippet here

