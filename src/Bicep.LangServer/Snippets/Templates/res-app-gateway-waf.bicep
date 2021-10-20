// Application Gateway with Web Application Firewall
resource /*${1:applicationGatewayFirewall}*/applicationGatewayFirewall 'Microsoft.Network/ApplicationGatewayWebApplicationFirewallPolicies@2020-11-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  properties: {
    policySettings: {
      requestBodyCheck: /*${3|true,false|}*/true
      maxRequestBodySizeInKb: /*${4:'maxRequestBodySizeInKb'}*/'maxRequestBodySizeInKb'
      fileUploadLimitInMb: /*${5:'fileUploadLimitInMb'}*/'fileUploadLimitInMb'
      state: /*'${6|Enabled,Disabled|}'*/'Enabled'
      mode: /*'${7|Detection,Prevention|}'*/'Detection'
    }
    managedRules: {
      managedRuleSets: [
        {
          ruleSetType: /*${8:'ruleSetType'}*/'ruleSetType'
          ruleSetVersion: /*${9:'ruleSetVersion'}*/'ruleSetVersion'
        }
      ]
    }
  }
}
