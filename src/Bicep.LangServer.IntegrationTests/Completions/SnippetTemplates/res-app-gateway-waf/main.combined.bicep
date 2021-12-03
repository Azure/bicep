// $1 = applicationGatewayFirewall
// $2 = 'name'
// $3 = true
// $4 = 128
// $5 = 100
// $6 = Enabled
// $7 = Detection
// $8 = 'ruleSetType'
// $9 = 'ruleSetVersion'

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

