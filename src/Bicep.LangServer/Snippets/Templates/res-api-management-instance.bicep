// Api Management Instance
resource ${1:apiManagementInstance} 'Microsoft.ApiManagement/service@2020-12-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  sku:{
    capacity: ${3|0,1|}
    name: ${4|'Developer','Consumption'|}
  }
  properties:{
    virtualNetworkType: ${5:'None'}
    publisherEmail: ${6:'publisherEmail@contoso.com'}
    publisherName: ${7:'publisherName'}
  }
}
