// $1 = apiManagementInstance
// $2 = 'name'
// $3 = 1
// $4 = 'Developer'
// $5 = 'None'
// $6 = 'publisherEmail@contoso.com'
// $7 = 'publisherName'

param location string

resource apiManagementInstance 'Microsoft.ApiManagement/service@2020-12-01' = {
  name: 'name'
  location: location
  sku:{
    capacity: 1
    name: 'Developer'
  }
  properties:{
    virtualNetworkType: 'None'
    publisherEmail: 'publisherEmail@contoso.com'
    publisherName: 'publisherName'
  }
}
// Insert snippet here

