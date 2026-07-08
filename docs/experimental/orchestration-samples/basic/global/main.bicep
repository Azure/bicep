param apps {
  subscriptionId: string
  resourceGroup: string
  name: string
}[]

param afdName string

resource containerApps 'Microsoft.App/containerApps@2025-02-02-preview' existing = [for app in apps: {
  scope: resourceGroup(app.subscriptionId, app.resourceGroup)
  name: app.name
}]

var afd = {
  endpointName: 'hello-world-endpoint'
  routeName: 'hello-world-route'
  originGroup: 'hello-world-group'
}

resource waf 'Microsoft.Network/FrontDoorWebApplicationFirewallPolicies@2024-02-01' = {
  name: replace(afdName, '-', '')
  location: 'global'
  sku: {
    name: 'Premium_AzureFrontDoor'
  }
  properties: {
    managedRules: {
      managedRuleSets: [
        {
          ruleSetType: 'Microsoft_BotManagerRuleSet'
          ruleSetVersion: '1.1'
        }
      ]
    }
    customRules: {
      rules: [
        {
          action: 'Block'
          enabledState: 'Enabled'
          matchConditions: [
            {
              matchValue: [
                '0.0.0.0/0'
              ]
              matchVariable: 'SocketAddr'
              negateCondition: false
              operator: 'IPMatch'
              transforms: []
            }
          ]
          name: 'GlobalRateLimitRule'
          priority: 100
          rateLimitDurationInMinutes: 5
          rateLimitThreshold: 1000
          ruleType: 'RateLimitRule'
        }
      ]
    }
    policySettings: {
      enabledState: 'Enabled'
      // TODO: Change to prevention when we confirm there are no false positives in the logs
      mode: 'Detection'
      requestBodyCheck: 'Enabled'
      customBlockResponseBody: null
      customBlockResponseStatusCode: 403
      redirectUrl: null
      javascriptChallengeExpirationInMinutes: 30
      logScrubbing: null
    }
  }
}

module profile 'br/public:avm/res/cdn/profile:0.14.0' = {
  params: {
    name: afdName
    sku: 'Premium_AzureFrontDoor'
    afdEndpoints: [
      {
        name: afd.endpointName
        routes: [
          {
            name: afd.routeName
            originGroupName: afd.originGroup
            ruleSets: []
          }
        ]
      }
    ]
    location: 'global'
    managedIdentities: {
      systemAssigned: true
    }
    originGroups: [
      {
        loadBalancingSettings: {
          additionalLatencyInMilliseconds: 50
          sampleSize: 4
          successfulSamplesRequired: 3
        }
        name: afd.originGroup
        origins: [for (app, i) in apps: {
          hostName: containerApps[i].properties.configuration.ingress.fqdn
          name: 'containerapp-${i}'
        }]
        healthProbeSettings: {
          probePath: '/'
          probeRequestType: 'GET'
          probeProtocol: 'Https'
          probeIntervalInSeconds: 60
        }
      }
    ]
    originResponseTimeoutSeconds: 60
    securityPolicies: [
      {
        name: 'ddos'
        associations: [
          {
            domains: [
              {
                id: resourceId('Microsoft.Cdn/profiles/afdEndpoints', afdName, afd.endpointName)
              }
            ]
            patternsToMatch: [
              '/*'
            ]
          }
        ]
        wafPolicyResourceId: waf.id
      }
    ]
  }
}
