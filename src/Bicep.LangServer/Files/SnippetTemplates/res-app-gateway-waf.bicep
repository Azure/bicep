// Application Gateway with Web Application Firewall
resource /*${1:applicationGatewayFirewall}*/applicationGatewayFirewall 'Microsoft.Network/ApplicationGatewayWebApplicationFirewallPolicies@2020-11-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: {
    policySettings: {
      requestBodyCheck: /*${4|true,false|}*/true
      maxRequestBodySizeInKb: /*${5:'maxRequestBodySizeInKb'}*/'maxRequestBodySizeInKb'
      fileUploadLimitInMb: /*${6:'fileUploadLimitInMb'}*/'fileUploadLimitInMb'
      state: /*'${7|Enabled,Disabled|}'*/'Enabled'
      mode: /*'${8|Detection,Prevention|}'*/'Detection'
    }
    managedRules: {
      managedRuleSets: [
        {
          ruleSetType: /*${9:'ruleSetType'}*/'ruleSetType'
          ruleSetVersion: /*${10:'ruleSetVersion'}*/'ruleSetVersion'
        }
      ]
    }
  }
}
