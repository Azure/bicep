// Azure Functions (v2)
resource /*${1:azureFunction}*/azureFunction 'Microsoft.Web/sites@2020-12-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  kind: 'functionapp'
  properties: {
    serverFarmId: /*${4:'serverfarms.id'}*/'serverfarms.id'
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsDashboard'
          value: /*'DefaultEndpointsProtocol=https;AccountName=${5:storageAccountName1};AccountKey=${listKeys(${6:'storageAccountID1'}, '2019-06-01').key1}'*/'value'
        }
        {
          name: 'AzureWebJobsStorage'
          value: /*'DefaultEndpointsProtocol=https;AccountName=${7:storageAccountName2};AccountKey=${listKeys(${8:'storageAccountID2'}, '2019-06-01').key1}'*/'value'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: /*'DefaultEndpointsProtocol=https;AccountName=${9:storageAccountName3};AccountKey=${listKeys(${10:'storageAccountID3'}, '2019-06-01').key1}'*/'value'
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: /*toLower(${11:'name'})*/'value'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~2'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: reference(/*${12:'insightsComponents.id'}*/'insightsComponents.id', '2015-05-01').InstrumentationKey
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: /*'${13|dotnet,node,java|}'*/'dotnet'
        }
      ]
    }
  }
}
