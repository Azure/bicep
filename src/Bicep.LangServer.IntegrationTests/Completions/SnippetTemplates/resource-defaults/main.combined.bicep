// $1 = nsg
// $2 = Microsoft.Aad/domainServices@2017-06-01
// $3 = 'testResource'
// $4 = location

param location string

resource nsg 'Microsoft.Aad/domainServices@2017-06-01' = {
  name: 'testResource'
  location: location
  properties: {
    
  }
}
// Insert snippet here

