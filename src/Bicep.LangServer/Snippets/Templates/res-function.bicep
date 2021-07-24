// Azure Functions (v2)
resource /*${1:serverfarms}*/serverfarms 'Microsoft.Web/serverfarms@2021-01-15' existing = {
  name: /*${2:'name'}*/'name'
}

resource /*${3:insightsComponents}*/insightsComponents 'Microsoft.Insights/components@2020-02-02' existing = {
  name: /*${4:'name'}*/'name'
}

resource /*${5:azureFunction}*/azureFunction 'Microsoft.Web/sites@2020-12-01' = {
  name: /*${6:'name'}*/'name'
  location: resourceGroup().location
  kind: 'functionapp'
  properties: {
    serverFarmId: serverfarms.id
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsDashboard'
          value: /*'DefaultEndpointsProtocol=https;AccountName=${7:storageAccountName1};AccountKey=${listKeys(${8:'storageAccountID1'}, '2019-06-01').key1}'*/'value'
        }
        {
          name: 'AzureWebJobsStorage'
          value: /*'DefaultEndpointsProtocol=https;AccountName=${9:storageAccountName2};AccountKey=${listKeys(${10:'storageAccountID2'}, '2019-06-01').key1}'*/'value'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: /*'DefaultEndpointsProtocol=https;AccountName=${11:storageAccountName3};AccountKey=${listKeys(${12:'storageAccountID3'}, '2019-06-01').key1}'*/'value'
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: /*toLower(${13:'name'})*/'value'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~2'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: reference(insightsComponents.id, '2015-05-01').InstrumentationKey
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: /*'${14|dotnet,node,java|}'*/'dotnet'
        }
      ]
    }
  }
}
