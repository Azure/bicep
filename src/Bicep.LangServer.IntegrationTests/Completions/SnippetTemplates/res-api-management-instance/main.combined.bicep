// $1 = apiManagementInstance
// $2 = 'name'
// $3 = location
// $4 = 1
// $5 = 'Developer'
// $6 = 'None'
// $7 = 'publisherEmail@contoso.com'
// $8 = 'publisherName'

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

