// ****************************************
// Azure Bicep Module:
// Create Name / Value pairs from sample JSON records
// ****************************************
@minLength(1)
param apimInstanceName string
//Name-Value Pair records
var apimNameValueSet = [
  {
    'displayName': 'NameValue1'
    'value': 'SomeValue1'
    'tags': [
      'Example'
    ]
    'isSecret': false
  }
  {
    'displayName': 'NameSecretValue'
    'value': 'SomeSecretValue'
    'tags': [
      'Example'
      'AnotherExampleTag'
    ]
    'isSecret': true
  }
]

//APIM name value pairs
resource apiManagementNVPair 'Microsoft.ApiManagement/service/namedValues@2020-06-01-preview' = [for nv in apimNameValueSet: {
  name: '${apimInstanceName}/${nv.displayName}'
  properties: {
    displayName: nv.displayName
    secret: nv.isSecret
    value: nv.value
    tags: nv.tags
  }
}]

output apimNameValues array = [for (nv, i) in apimNameValueSet: {
  nameValueId: apiManagementNVPair[i].id
  naameValueName: apiManagementNVPair[i].name
}]
