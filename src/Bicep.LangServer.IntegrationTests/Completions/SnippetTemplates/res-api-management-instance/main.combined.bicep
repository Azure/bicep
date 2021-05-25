resource apiManagementInstance 'Microsoft.ApiManagement/service@2020-12-01' = {
  name: 'name'
  location: resourceGroup().location
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

