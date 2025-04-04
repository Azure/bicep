// BEGIN: Parameters

param strParam1 string

@secure()
param secureStrParam1 string

param boolParam1 bool

// END: Parameters

// BEGIN: Extension declarations

extension kubernetes with {
  kubeConfig: 'DELETE'
  namespace: 'DELETE'
} as k8s

//extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1:1.2.3' as graph

// END: Extension declarations

// BEGIN: Key vaults

resource kv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'kv1'
}

resource scopedKv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'scopedKv1'
  scope: resourceGroup('otherGroup')
}

resource testResource1 'az:My.Rp/TestType@2020-01-01' = {
  name: k8s.config.namespace
  properties: {
    secret: k8s.config.kubeConfig
  }
}

// END: Key vaults

// BEGIN: Extension configs for modules

module moduleWithExtsWithAliases 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  name: 'moduleWithExtsWithAliases'
  extensionConfigs: {
    k8s: {
      kubeConfig: 'kubeConfig2FromModule'
      namespace: 'ns2FromModule'
    }
  }
}

module moduleWithExtsWithoutAliases 'child/hasConfigurableExtensionsWithoutAlias.bicep' = {
  name: 'moduleWithExtsWithoutAliases'
  extensionConfigs: {
    kubernetes: {
      kubeConfig: 'kubeConfig2FromModule'
      namespace: 'ns2FromModule'
    }
  }
}

module moduleExtConfigsFromParams 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  name: 'moduleExtConfigsFromParams'
  extensionConfigs: {
    k8s: {
      kubeConfig: boolParam1 ? secureStrParam1 : strParam1
      namespace: boolParam1 ? strParam1 : 'falseCond'
    }
  }
}

module moduleExtConfigFromKeyVaultReference 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  name: 'moduleExtConfigKeyVaultReference'
  extensionConfigs: {
    k8s: {
      kubeConfig: kv1.getSecret('myKubeConfig')
      namespace: 'default'
    }
  }
}

module moduleWithExtsUsingFullInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  name: 'moduleWithExtsFullInheritance'
  extensionConfigs: {
    k8s: k8s.config
  }
}

module moduleWithExtsUsingPiecemealInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  name: 'moduleWithExtsPiecemealInheritance'
  extensionConfigs: {
    k8s: {
      kubeConfig: k8s.config.kubeConfig
      namespace: k8s.config.namespace
    }
  }
}

// TODO(kylealbert): Figure out if this is allowable
// var k8sConfigDeployTime = {
//   kubeConfig: k8s.config.kubeConfig
//   namespace: strParam1
// }

// module moduleWithExtsUsingVar 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//   name: 'moduleWithExtsUsingVar'
//   extensionConfigs: {
//     k8s: k8sConfigDeployTime
//   }
// }

// END: Extension configs for modules

// BEGIN: Outputs

output k8sConfig object = k8s.config

output k8sNamespace string = k8s.config.namespace

// END: Outputs
