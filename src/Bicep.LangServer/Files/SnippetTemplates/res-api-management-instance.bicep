// Api Management Instance
resource /*${1:apiManagementInstance}*/apiManagementInstance 'Microsoft.ApiManagement/service@2020-12-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  sku:{
    capacity: /*${4|0,1|}*/0
    name: /*${5|'Developer','Consumption'|}*/'Developer'
  }
  properties:{
    virtualNetworkType: /*${6:'None'}*/'None'
    publisherEmail: /*${7:'publisherEmail@contoso.com'}*/'publisherEmail@contoso.com'
    publisherName: /*${8:'publisherName'}*/'publisherName'
  }
}
