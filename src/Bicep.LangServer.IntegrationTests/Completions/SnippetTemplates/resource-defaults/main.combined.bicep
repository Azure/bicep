// $1 = nsg
// $2 = Microsoft.Aad/domainServices@2017-06-01
// $3 = 'testResource'
// $4 = 'testLocation'

resource nsg 'Microsoft.Aad/domainServices@2017-06-01' = {
  name: 'testResource'
  location: 'testLocation'
//@[12:26) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'testLocation' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'testLocation'|
  properties: {
    
  }
}// Insert snippet here

