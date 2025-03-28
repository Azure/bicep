// BEGIN: Extension declarations
//@[00:1330) ProgramExpression

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

//extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0:0.1.8-preview' as graph

// END: Extension declarations

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
//@[00:0275) └─DeclaredModuleExpression
//@[98:0275)   ├─ObjectExpression
  name: 'moduleWithExtsPiecemealInheritance'
//@[02:0044)   | └─ObjectPropertyExpression
//@[02:0006)   |   ├─StringLiteralExpression { Value = name }
//@[08:0044)   |   └─StringLiteralExpression { Value = moduleWithExtsPiecemealInheritance }
  extensionConfigs: {
//@[20:0125)   └─ObjectExpression
    k8s: {
//@[04:0097)     └─ObjectPropertyExpression
//@[04:0007)       ├─StringLiteralExpression { Value = k8s }
//@[09:0097)       └─ObjectExpression
      kubeConfig: k8s.config.kubeConfig
//@[06:0039)         ├─ObjectPropertyExpression
//@[06:0016)         | ├─StringLiteralExpression { Value = kubeConfig }
//@[18:0039)         | └─PropertyAccessExpression { PropertyName = kubeConfig }
//@[18:0028)         |   └─PropertyAccessExpression { PropertyName = config }
//@[18:0021)         |     └─ExtensionReferenceExpression { ExtensionAlias = k8s }
      namespace: k8s.config.namespace
//@[06:0037)         └─ObjectPropertyExpression
//@[06:0015)           ├─StringLiteralExpression { Value = namespace }
//@[17:0037)           └─PropertyAccessExpression { PropertyName = namespace }
//@[17:0027)             └─PropertyAccessExpression { PropertyName = config }
//@[17:0020)               └─ExtensionReferenceExpression { ExtensionAlias = k8s }
    }
  }
}

// END: Extension configs for modules

