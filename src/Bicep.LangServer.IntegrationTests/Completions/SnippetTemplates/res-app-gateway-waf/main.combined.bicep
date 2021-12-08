// $1 = applicationGatewayFirewall
// $2 = 'name'
// $3 = location
// $4 = true
// $5 = 128
// $6 = 100
// $7 = Enabled
// $8 = Detection
// $9 = 'ruleSetType'
// $10 = 'ruleSetVersion'

param location string

resource applicationGatewayFirewall 'Microsoft.Network/ApplicationGatewayWebApplicationFirewallPolicies@2020-11-01' = {
  name: 'name'
  location: location
  properties: {
    policySettings: {
      requestBodyCheck: true
      maxRequestBodySizeInKb: 128
      fileUploadLimitInMb: 100
      state: 'Enabled'
      mode: 'Detection'
    }
    managedRules: {
      managedRuleSets: [
        {
          ruleSetType: 'ruleSetType'
          ruleSetVersion: 'ruleSetVersion'
        }
      ]
    }
  }
}
// Insert snippet here

