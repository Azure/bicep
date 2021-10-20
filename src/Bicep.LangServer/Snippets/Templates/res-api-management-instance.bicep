// Api Management Instance
resource /*${1:apiManagementInstance}*/apiManagementInstance 'Microsoft.ApiManagement/service@2020-12-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  sku:{
    capacity: /*${3|0,1|}*/0
    name: /*${4|'Developer','Consumption'|}*/'Developer'
  }
  properties:{
    virtualNetworkType: /*${5:'None'}*/'None'
    publisherEmail: /*${6:'publisherEmail@contoso.com'}*/'publisherEmail@contoso.com'
    publisherName: /*${7:'publisherName'}*/'publisherName'
  }
}
