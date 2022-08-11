// Logic App Integration Account
resource /*${1:logicAppIntegrationAccount}*/logicAppIntegrationAccount 'Microsoft.Logic/integrationAccounts@2019-05-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
}
