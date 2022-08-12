// $1 = azureFunction
// $2 = 'name'
// $3 = location
// $4 = 'serverfarms.id'
// $5 = storageAccountName1
// $6 = 'storageAccountID1'
// $7 = storageAccountName2
// $8 = 'storageAccountID2'
// $9 = storageAccountName3
// $10 = 'storageAccountID3'
// $11 = 'name'
// $12 = 'insightsComponents'
// $13 = dotnet

param location string

resource azureFunction 'Microsoft.Web/sites@2020-12-01' = {
  name: 'name'
  location: location
  kind: 'functionapp'
  properties: {
    serverFarmId: 'serverfarms.id'
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
          value: reference('insightsComponents', '2015-05-01').InstrumentationKey
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

