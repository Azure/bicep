param frontDoorName string = 'AzFd-TestingBicep-999'
param frontDoorEnabledState bool = true
param healthProbe1EnabledState bool = false
param frontDoorWafDeployed bool = false // Used for conditions once available in bicep 0.3
param frontDoorWafNamePrefix string = 'AzFdWafTestingBicep'
param frontDoorWafEnabledState bool = true

@allowed([
  'Prevention'
  'Detection'
])
param frontDoorWafMode string = 'Prevention'

var frontDoorNameLower = toLower(frontDoorName)
var backendPool1Name = '${frontDoorNameLower}-backendPool1'

var healthProbe1Name = '${frontDoorNameLower}-healthProbe1'
var frontendEndpoint1Name = '${frontDoorNameLower}-frontendEndpoint1'
var loadBalancing1Name = '${frontDoorNameLower}-loadBalancing1'
var routingRule1Name = '${frontDoorNameLower}-routingRule1'
var routingRule2Name = '${frontDoorNameLower}-routingRule2'

var frontendEndpoint1hostName = '${frontDoorNameLower}.azurefd.net'
var backendExampleTarget = 'api.myip.com'
var redirectExampleTarget = 'api.myip.com'

var frontDoorWafName = '${frontDoorWafNamePrefix}${uniqueString(subscription().subscriptionId, resourceGroup().id, frontDoorWafNamePrefix)}'

resource resAzFd 'Microsoft.Network/frontdoors@2020-01-01' = {
  name: frontDoorNameLower
  location: 'Global'
  properties: {
    enabledState: frontDoorEnabledState ? 'Enabled' : 'Disabled'
    friendlyName: frontDoorNameLower
    frontendEndpoints: [
      {
        name: frontendEndpoint1Name
        properties: {
          hostName: frontendEndpoint1hostName
          sessionAffinityEnabledState: 'Disabled'
          sessionAffinityTtlSeconds: 0
          webApplicationFirewallPolicyLink: {
            id: '${resAzFdWaf.id}'
          }
        }
      }
    ]
    backendPoolsSettings: {
      enforceCertificateNameCheck: 'Enabled'
      sendRecvTimeoutSeconds: 30
    }
    backendPools: [
      {
        name: backendPool1Name
        properties: {
          backends: [
            {
              address: backendExampleTarget
              backendHostHeader: backendExampleTarget
              enabledState: 'Enabled'
              httpPort: 80
              httpsPort: 443
              priority: 1
              weight: 50
            }
          ]
          healthProbeSettings: {
            id: resourceId('Microsoft.Network/frontDoors/healthProbeSettings', frontDoorNameLower, healthProbe1Name)
          }
          loadBalancingSettings: {
            id: resourceId('Microsoft.Network/frontDoors/LoadBalancingSettings', frontDoorNameLower, loadBalancing1Name)
          }
        }
      }
    ]
    healthProbeSettings: [
      {
        name: healthProbe1Name
        properties: {
          enabledState: healthProbe1EnabledState ? 'Enabled' : 'Disabled'
          intervalInSeconds: 30
          path: '/'
          protocol: 'Https'
          healthProbeMethod: 'HEAD'
        }
      }
    ]
    loadBalancingSettings: [
      {
        name: loadBalancing1Name
        properties: {
          additionalLatencyMilliseconds: 0
          sampleSize: 4
          successfulSamplesRequired: 2
        }
      }
    ]
    routingRules: [
      {
        name: routingRule1Name
        properties: {
          frontendEndpoints: [
            {
              id: resourceId('Microsoft.Network/frontDoors/FrontendEndpoints', frontDoorNameLower, frontendEndpoint1Name)
            }
          ]
          acceptedProtocols: [
            'Https'
          ]
          patternsToMatch: [
            '/*'
          ]
          enabledState: 'Enabled'
          routeConfiguration: {
            '@odata.type': '#Microsoft.Azure.FrontDoor.Models.FrontdoorForwardingConfiguration'
            forwardingProtocol: 'HttpsOnly'
            backendPool: {
              id: resourceId('Microsoft.Network/frontDoors/BackendPools', frontDoorNameLower, backendPool1Name)
            }
          }
        }
      }
      {
        name: routingRule2Name
        properties: {
          frontendEndpoints: [
            {
              id: resourceId('Microsoft.Network/frontDoors/FrontendEndpoints', frontDoorNameLower, frontendEndpoint1Name)
            }
          ]
          acceptedProtocols: [
            'Https'
          ]
          patternsToMatch: [
            '/redirect/*'
            '/redirect'
          ]
          enabledState: 'Enabled'
          routeConfiguration: {
            '@odata.type': '#Microsoft.Azure.FrontDoor.Models.FrontdoorRedirectConfiguration'
            customHost: redirectExampleTarget
            customPath: '/'
            redirectProtocol: 'HttpsOnly'
            redirectType: 'Found'
          }
        }
      }
    ]
  }
}

resource resAzFdWaf 'Microsoft.Network/FrontDoorWebApplicationFirewallPolicies@2019-10-01' = {
  name: frontDoorWafName
  location: 'Global'
  properties: {
    policySettings: {
      enabledState: frontDoorWafEnabledState ? 'Enabled' : 'Disabled'
      mode: frontDoorWafMode
      customBlockResponseStatusCode: 403
    }
    customRules: {
      rules: []
    }
    managedRules: {
      managedRuleSets: [
        {
          ruleSetType: 'DefaultRuleSet'
          ruleSetVersion: '1.0'
          ruleGroupOverrides: []
          exclusions: []
        }
      ]
    }
  }
}
