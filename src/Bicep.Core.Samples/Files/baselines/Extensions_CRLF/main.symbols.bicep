// BEGIN: Parameters

param strParam1 string
//@[6:15) Parameter strParam1. Type: string. Declaration start char: 0, length: 22

@secure()
param secureStrParam1 string
//@[6:21) Parameter secureStrParam1. Type: string. Declaration start char: 0, length: 39

param boolParam1 bool
//@[6:16) Parameter boolParam1. Type: bool. Declaration start char: 0, length: 21

// END: Parameters

// BEGIN: Extension declarations

extension kubernetes with {
  kubeConfig: 'DELETE'
  namespace: 'DELETE'
} as k8s
//@[5:08) ImportedNamespace k8s. Type: k8s. Declaration start char: 0, length: 84

//extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1:1.2.3' as graph

// END: Extension declarations

// BEGIN: Key vaults

resource kv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[9:12) Resource kv1. Type: Microsoft.KeyVault/vaults@2019-09-01. Declaration start char: 0, length: 82
  name: 'kv1'
}

resource scopedKv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[9:18) Resource scopedKv1. Type: Microsoft.KeyVault/vaults@2019-09-01. Declaration start char: 0, length: 132
  name: 'scopedKv1'
  scope: resourceGroup('otherGroup')
}

resource testResource1 'az:My.Rp/TestType@2020-01-01' = {
//@[9:22) Resource testResource1. Type: My.Rp/TestType@2020-01-01. Declaration start char: 0, length: 147
  name: k8s.config.namespace
  properties: {
    secret: k8s.config.kubeConfig
  }
}

// END: Key vaults

// BEGIN: Extension configs for modules

module moduleWithExtsWithAliases 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[7:32) Module moduleWithExtsWithAliases. Type: module. Declaration start char: 0, length: 249
  name: 'moduleWithExtsWithAliases'
  extensionConfigs: {
    k8s: {
      kubeConfig: 'kubeConfig2FromModule'
      namespace: 'ns2FromModule'
    }
  }
}

module moduleWithExtsWithoutAliases 'child/hasConfigurableExtensionsWithoutAlias.bicep' = {
//@[7:35) Module moduleWithExtsWithoutAliases. Type: module. Declaration start char: 0, length: 265
  name: 'moduleWithExtsWithoutAliases'
  extensionConfigs: {
    kubernetes: {
      kubeConfig: 'kubeConfig2FromModule'
      namespace: 'ns2FromModule'
    }
  }
}

module moduleExtConfigsFromParams 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[7:33) Module moduleExtConfigsFromParams. Type: module. Declaration start char: 0, length: 289
  name: 'moduleExtConfigsFromParams'
  extensionConfigs: {
    k8s: {
      kubeConfig: boolParam1 ? secureStrParam1 : strParam1
      namespace: boolParam1 ? strParam1 : 'falseCond'
    }
  }
}

// TODO(kylealbert): Allow key vault references in extension configs
// module moduleExtConfigFromKeyVaultReference 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//   name: 'moduleExtConfigKeyVaultReference'
//   extensionConfigs: {
//     k8s: {
//       kubeConfig: kv1.getSecret('myKubeConfig'),
//       namespace: scopedKv1.getSecret('myNamespace')
//     }
//   }
// }

module moduleWithExtsUsingFullInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[7:41) Module moduleWithExtsUsingFullInheritance. Type: module. Declaration start char: 0, length: 187
  name: 'moduleWithExtsFullInheritance'
  extensionConfigs: {
    k8s: k8s.config
  }
}

module moduleWithExtsUsingPiecemealInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[7:46) Module moduleWithExtsUsingPiecemealInheritance. Type: module. Declaration start char: 0, length: 275
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
//@[7:16) Output k8sConfig. Type: object. Declaration start char: 0, length: 36

output k8sNamespace string = k8s.config.namespace
//@[7:19) Output k8sNamespace. Type: string. Declaration start char: 0, length: 49

// END: Outputs

