// Logic App Integration Account
resource /*${1:logicAppIntegrationAccount}*/logicAppIntegrationAccount 'Microsoft.Logic/integrationAccounts@2016-06-01' = {
  name: /*${2:'name'}*/'name'
  location: location
}
