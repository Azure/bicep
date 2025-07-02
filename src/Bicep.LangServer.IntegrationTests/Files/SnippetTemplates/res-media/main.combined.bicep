// $1 = mediaServices
// $2 = 'name'
// $3 = location
// $4 = 'storageAccount.id'
// $5 = Primary

param location string

resource mediaServices 'Microsoft.Media/mediaServices@2020-05-01' = {
//@[23:65) [BCP081 (Warning)] Resource type "Microsoft.Media/mediaServices@2020-05-01" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed. (bicep https://aka.ms/bicep/core-diagnostics#BCP081) |'Microsoft.Media/mediaServices@2020-05-01'|
  name: 'name'
  location: location
  properties: {
    storageAccounts: [
      {
        id: 'storageAccount.id'
        type: 'Primary'
      }
    ]
  }
}


