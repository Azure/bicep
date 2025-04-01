// BEGIN: Parameters
//@[00:3059) ProgramExpression

param strParam1 string
//@[00:0022) ├─DeclaredParameterExpression { Name = strParam1 }
//@[16:0022) | └─AmbientTypeReferenceExpression { Name = string }

@secure()
//@[00:0039) ├─DeclaredParameterExpression { Name = secureStrParam1 }
//@[01:0009) | ├─FunctionCallExpression { Name = secure }
param secureStrParam1 string
//@[22:0028) | └─AmbientTypeReferenceExpression { Name = string }

param boolParam1 bool
//@[00:0021) ├─DeclaredParameterExpression { Name = boolParam1 }
//@[17:0021) | └─AmbientTypeReferenceExpression { Name = bool }

// END: Parameters

// BEGIN: Extension declarations

extension kubernetes with {
//@[00:0084) ├─ExtensionExpression { Name = k8s }
//@[26:0077) | └─ObjectExpression
  kubeConfig: 'DELETE'
//@[02:0022) |   ├─ObjectPropertyExpression
//@[02:0012) |   | ├─StringLiteralExpression { Value = kubeConfig }
//@[14:0022) |   | └─StringLiteralExpression { Value = DELETE }
  namespace: 'DELETE'
//@[02:0021) |   └─ObjectPropertyExpression
//@[02:0011) |     ├─StringLiteralExpression { Value = namespace }
//@[13:0021) |     └─StringLiteralExpression { Value = DELETE }
} as k8s

//extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1:1.2.3' as graph

// END: Extension declarations

// BEGIN: Key vaults

resource kv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[00:0082) ├─DeclaredResourceExpression
//@[63:0082) | └─ObjectExpression
  name: 'kv1'
}

resource scopedKv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[00:0132) ├─DeclaredResourceExpression
//@[69:0132) | └─ObjectExpression
  name: 'scopedKv1'
  scope: resourceGroup('otherGroup')
}

resource testResource1 'az:My.Rp/TestType@2020-01-01' = {
//@[00:0147) ├─DeclaredResourceExpression
//@[56:0147) | └─ObjectExpression
  name: k8s.config.namespace
  properties: {
//@[02:0055) |   └─ObjectPropertyExpression
//@[02:0012) |     ├─StringLiteralExpression { Value = properties }
//@[14:0055) |     └─ObjectExpression
    secret: k8s.config.kubeConfig
//@[04:0033) |       └─ObjectPropertyExpression
//@[04:0010) |         ├─StringLiteralExpression { Value = secret }
//@[12:0033) |         └─PropertyAccessExpression { PropertyName = kubeConfig }
//@[12:0022) |           └─PropertyAccessExpression { PropertyName = config }
//@[12:0015) |             └─ExtensionReferenceExpression { ExtensionAlias = k8s }
  }
}

// END: Key vaults

// BEGIN: Extension configs for modules

module moduleWithExtsWithAliases 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[00:0249) ├─DeclaredModuleExpression
//@[84:0249) | ├─ObjectExpression
  name: 'moduleWithExtsWithAliases'
//@[02:0035) | | └─ObjectPropertyExpression
//@[02:0006) | |   ├─StringLiteralExpression { Value = name }
//@[08:0035) | |   └─StringLiteralExpression { Value = moduleWithExtsWithAliases }
  extensionConfigs: {
//@[20:0122) | └─ObjectExpression
    k8s: {
//@[04:0094) |   └─ObjectPropertyExpression
//@[04:0007) |     ├─StringLiteralExpression { Value = k8s }
//@[09:0094) |     └─ObjectExpression
      kubeConfig: 'kubeConfig2FromModule'
//@[06:0041) |       ├─ObjectPropertyExpression
//@[06:0016) |       | ├─StringLiteralExpression { Value = kubeConfig }
//@[18:0041) |       | └─StringLiteralExpression { Value = kubeConfig2FromModule }
      namespace: 'ns2FromModule'
//@[06:0032) |       └─ObjectPropertyExpression
//@[06:0015) |         ├─StringLiteralExpression { Value = namespace }
//@[17:0032) |         └─StringLiteralExpression { Value = ns2FromModule }
    }
  }
}

module moduleWithExtsWithoutAliases 'child/hasConfigurableExtensionsWithoutAlias.bicep' = {
//@[00:0265) ├─DeclaredModuleExpression
//@[90:0265) | ├─ObjectExpression
  name: 'moduleWithExtsWithoutAliases'
//@[02:0038) | | └─ObjectPropertyExpression
//@[02:0006) | |   ├─StringLiteralExpression { Value = name }
//@[08:0038) | |   └─StringLiteralExpression { Value = moduleWithExtsWithoutAliases }
  extensionConfigs: {
//@[20:0129) | └─ObjectExpression
    kubernetes: {
//@[04:0101) |   └─ObjectPropertyExpression
//@[04:0014) |     ├─StringLiteralExpression { Value = kubernetes }
//@[16:0101) |     └─ObjectExpression
      kubeConfig: 'kubeConfig2FromModule'
//@[06:0041) |       ├─ObjectPropertyExpression
//@[06:0016) |       | ├─StringLiteralExpression { Value = kubeConfig }
//@[18:0041) |       | └─StringLiteralExpression { Value = kubeConfig2FromModule }
      namespace: 'ns2FromModule'
//@[06:0032) |       └─ObjectPropertyExpression
//@[06:0015) |         ├─StringLiteralExpression { Value = namespace }
//@[17:0032) |         └─StringLiteralExpression { Value = ns2FromModule }
    }
  }
}

module moduleExtConfigsFromParams 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[00:0289) ├─DeclaredModuleExpression
//@[85:0289) | ├─ObjectExpression
  name: 'moduleExtConfigsFromParams'
//@[02:0036) | | └─ObjectPropertyExpression
//@[02:0006) | |   ├─StringLiteralExpression { Value = name }
//@[08:0036) | |   └─StringLiteralExpression { Value = moduleExtConfigsFromParams }
  extensionConfigs: {
//@[20:0160) | └─ObjectExpression
    k8s: {
//@[04:0132) |   └─ObjectPropertyExpression
//@[04:0007) |     ├─StringLiteralExpression { Value = k8s }
//@[09:0132) |     └─ObjectExpression
      kubeConfig: boolParam1 ? secureStrParam1 : strParam1
//@[06:0058) |       ├─ObjectPropertyExpression
//@[06:0016) |       | ├─StringLiteralExpression { Value = kubeConfig }
//@[18:0058) |       | └─TernaryExpression
//@[18:0028) |       |   ├─ParametersReferenceExpression { Parameter = boolParam1 }
//@[31:0046) |       |   ├─ParametersReferenceExpression { Parameter = secureStrParam1 }
//@[49:0058) |       |   └─ParametersReferenceExpression { Parameter = strParam1 }
      namespace: boolParam1 ? strParam1 : 'falseCond'
//@[06:0053) |       └─ObjectPropertyExpression
//@[06:0015) |         ├─StringLiteralExpression { Value = namespace }
//@[17:0053) |         └─TernaryExpression
//@[17:0027) |           ├─ParametersReferenceExpression { Parameter = boolParam1 }
//@[30:0039) |           ├─ParametersReferenceExpression { Parameter = strParam1 }
//@[42:0053) |           └─StringLiteralExpression { Value = falseCond }
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
//@[00:0187) ├─DeclaredModuleExpression
//@[93:0187) | ├─ObjectExpression
  name: 'moduleWithExtsFullInheritance'
//@[02:0039) | | └─ObjectPropertyExpression
//@[02:0006) | |   ├─StringLiteralExpression { Value = name }
//@[08:0039) | |   └─StringLiteralExpression { Value = moduleWithExtsFullInheritance }
  extensionConfigs: {
//@[20:0047) | └─ObjectExpression
    k8s: k8s.config
//@[04:0019) |   └─ObjectPropertyExpression
//@[04:0007) |     ├─StringLiteralExpression { Value = k8s }
//@[09:0019) |     └─PropertyAccessExpression { PropertyName = config }
//@[09:0012) |       └─ExtensionReferenceExpression { ExtensionAlias = k8s }
  }
}

module moduleWithExtsUsingPiecemealInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[00:0275) ├─DeclaredModuleExpression
//@[98:0275) | ├─ObjectExpression
  name: 'moduleWithExtsPiecemealInheritance'
//@[02:0044) | | └─ObjectPropertyExpression
//@[02:0006) | |   ├─StringLiteralExpression { Value = name }
//@[08:0044) | |   └─StringLiteralExpression { Value = moduleWithExtsPiecemealInheritance }
  extensionConfigs: {
//@[20:0125) | └─ObjectExpression
    k8s: {
//@[04:0097) |   └─ObjectPropertyExpression
//@[04:0007) |     ├─StringLiteralExpression { Value = k8s }
//@[09:0097) |     └─ObjectExpression
      kubeConfig: k8s.config.kubeConfig
//@[06:0039) |       ├─ObjectPropertyExpression
//@[06:0016) |       | ├─StringLiteralExpression { Value = kubeConfig }
//@[18:0039) |       | └─PropertyAccessExpression { PropertyName = kubeConfig }
//@[18:0028) |       |   └─PropertyAccessExpression { PropertyName = config }
//@[18:0021) |       |     └─ExtensionReferenceExpression { ExtensionAlias = k8s }
      namespace: k8s.config.namespace
//@[06:0037) |       └─ObjectPropertyExpression
//@[06:0015) |         ├─StringLiteralExpression { Value = namespace }
//@[17:0037) |         └─PropertyAccessExpression { PropertyName = namespace }
//@[17:0027) |           └─PropertyAccessExpression { PropertyName = config }
//@[17:0020) |             └─ExtensionReferenceExpression { ExtensionAlias = k8s }
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
//@[00:0036) ├─DeclaredOutputExpression { Name = k8sConfig }
//@[17:0023) | ├─AmbientTypeReferenceExpression { Name = object }
//@[26:0036) | └─PropertyAccessExpression { PropertyName = config }
//@[26:0029) |   └─ExtensionReferenceExpression { ExtensionAlias = k8s }

output k8sNamespace string = k8s.config.namespace
//@[00:0049) └─DeclaredOutputExpression { Name = k8sNamespace }
//@[20:0026)   ├─AmbientTypeReferenceExpression { Name = string }
//@[29:0049)   └─PropertyAccessExpression { PropertyName = namespace }
//@[29:0039)     └─PropertyAccessExpression { PropertyName = config }
//@[29:0032)       └─ExtensionReferenceExpression { ExtensionAlias = k8s }

// END: Outputs

