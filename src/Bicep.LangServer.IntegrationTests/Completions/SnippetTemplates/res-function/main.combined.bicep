// $1 = serverfarms
// $2 = 'name'
// $3 = insightsComponents
// $4 = 'name'
// $5 = azureFunction
// $6 = 'name'
// $7 = storageAccountName1
// $8 = 'storageAccountID1'
// $9 = storageAccountName2
// $10 = 'storageAccountID2'
// $11 = storageAccountName3
// $12 = 'storageAccountID3'
// $13 = 'name'
// $14 = dotnet


resource serverfarms 'Microsoft.Web/serverfarms@2021-01-15' existing = {
  name: 'name'
}

resource insightsComponents 'Microsoft.Insights/components@2020-02-02' existing = {
  name: 'name'
}

resource azureFunction 'Microsoft.Web/sites@2020-12-01' = {
  name: 'name'
  location: resourceGroup().location
  kind: 'functionapp'
  properties: {
    serverFarmId: serverfarms.id
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsDashboard'
          value: 'DefaultEndpointsProtocol=https;AccountName=storageAccountName1;AccountKey=${listKeys('storageAccountID1', '2019-06-01').key1}'
        }
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=storageAccountName2;AccountKey=${listKeys('storageAccountID2', '2019-06-01').key1}'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=storageAccountName3;AccountKey=${listKeys('storageAccountID3', '2019-06-01').key1}'
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: toLower('name')
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
          value: 'dotnet'
        }
      ]
    }
  }
}
// Insert snippet here
