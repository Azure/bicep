// $1 = mySQLdb
// $2 = 'name'
// $3 = location
// $4 = 'administratorLogin'
// $5 = 'administratorLoginPassword'
// $6 = 'Default'

param location string

resource mySQLdb 'Microsoft.DBforMySQL/servers@2017-12-01' = {
  name: 'name'
  location: location
  properties: {
    administratorLogin: 'administratorLogin'
    administratorLoginPassword: 'administratorLoginPassword'
//@[32:60) [BCP418 (Info)] The assignment target is expecting sensitive data but has been provided a non-sensitive value. Consider supplying the value as a secure parameter instead to prevent unauthorized disclosure to users who can view the template (via the portal, the CLI, or in source code). (bicep https://aka.ms/bicep/core-diagnostics#BCP418) |'administratorLoginPassword'|
    createMode: 'Default'
  }
}


