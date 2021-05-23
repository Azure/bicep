// Application Gateway with Web Application Firewall
resource ${1:applicationGatewayFirewall} 'Microsoft.Network/ApplicationGatewayWebApplicationFirewallPolicies@2020-11-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  properties: {
    policySettings: {
      requestBodyCheck: ${3|true,false|}
      maxRequestBodySizeInKb: ${4:'maxRequestBodySizeInKb'}
      fileUploadLimitInMb: ${5:'fileUploadLimitInMb'}
      state: '${6|Enabled,Disabled|}'
      mode: '${7|Detection,Prevention|}'
    }
    managedRules: {
      managedRuleSets: [
        {
          ruleSetType: ${8:'ruleSetType'}
          ruleSetVersion: ${9:'ruleSetVersion'}
        }
      ]
    }
  }
}
