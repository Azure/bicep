// Azure Functions (v2)
resource /*${1:azureFunction}*/azureFunction 'Microsoft.Web/sites@2020-12-01' = {
  name: /*${2:'name'}*/'name'
  location: resourceGroup().location
  kind: 'functionapp'
  properties: {
    serverFarmId: resourceId('Microsoft.Web/serverfarms', /*${3:'serverFarmName'}*/'serverFarmName')
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsDashboard'
          value: 'DefaultEndpointsProtocol=https;AccountName=/*${4:storageAccountName1}*/storageAccountName1;AccountKey=${listKeys(${5:'storageAccountID1'}, '2019-06-01').key1}'*/'value'
        }
        {
          name: 'AzureWebJobsStorage'
          value: /*'DefaultEndpointsProtocol=https;AccountName=${6:storageAccountName2};AccountKey=${listKeys(${7:'storageAccountID2'}, '2019-06-01').key1}'*/'value'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: /*'DefaultEndpointsProtocol=https;AccountName=${8:storageAccountName3};AccountKey=${listKeys(${9:'storageAccountID3'}, '2019-06-01').key1}'*/'value'
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: /*toLower(${2:'name'})*/'value'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~2'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: reference(resourceId('microsoft.insights/components/', /*${10:'applicationInsightsName'}*/'applicationInsightsName'), '2015-05-01').InstrumentationKey
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: /*'${11|dotnet,node,java|}'*/'dotnet'
        }
      ]
    }
  }
}
