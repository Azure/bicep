// Azure Functions (v2)
resource ${1:azureFunction} 'Microsoft.Web/sites@2018-11-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  kind: 'functionapp'
  properties: {
    serverFarmId: resourceId('Microsoft.Web/serverfarms', ${3:'serverFarmName'})
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsDashboard'
          value: 'DefaultEndpointsProtocol=https;AccountName=${4:storageAccountName};AccountKey=${listKeys(${5:'storageAccountID'}, '2015-05-01-preview').key1}'
        }
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${4:storageAccountName};AccountKey=${listKeys(${5:'storageAccountID'}, '2015-05-01-preview').key1}'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${4:storageAccountName};AccountKey=${listKeys(${5:'storageAccountID}, '2015-05-01-preview').key1}'
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: toLower(${2:'name'})
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~2'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: reference(resourceId('microsoft.insights/components/', ${6:'applicationInsightsName'}), '2015-05-01').InstrumentationKey
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: '${7|dotnet,node,java|}'
        }
      ]
    }
  }
}
