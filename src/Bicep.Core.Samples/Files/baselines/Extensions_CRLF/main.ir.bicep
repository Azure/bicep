// BEGIN: Extension declarations
//@[00:872) ProgramExpression

// extension kubernetes with {
//   kubeConfig: 'DELETE'
//   namespace: 'DELETE'
// } as k8s

//extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0:0.1.8-preview' as graph

// END: Extension declarations

// BEGIN: Extension configs for modules

module moduleWithExtsWithAliases 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[00:249) ├─DeclaredModuleExpression
//@[84:249) | ├─ObjectExpression
  name: 'moduleWithExtsWithAliases'
//@[02:035) | | └─ObjectPropertyExpression
//@[02:006) | |   ├─StringLiteralExpression { Value = name }
//@[08:035) | |   └─StringLiteralExpression { Value = moduleWithExtsWithAliases }
  extensionConfigs: {
//@[20:122) | └─ObjectExpression
    k8s: {
//@[04:094) |   └─ObjectPropertyExpression
//@[04:007) |     ├─StringLiteralExpression { Value = k8s }
//@[09:094) |     └─ObjectExpression
      kubeConfig: 'kubeConfig2FromModule'
//@[06:041) |       ├─ObjectPropertyExpression
//@[06:016) |       | ├─StringLiteralExpression { Value = kubeConfig }
//@[18:041) |       | └─StringLiteralExpression { Value = kubeConfig2FromModule }
      namespace: 'ns2FromModule'
//@[06:032) |       └─ObjectPropertyExpression
//@[06:015) |         ├─StringLiteralExpression { Value = namespace }
//@[17:032) |         └─StringLiteralExpression { Value = ns2FromModule }
    }
  }
}

module moduleWithExtsWithoutAliases 'child/hasConfigurableExtensionsWithoutAlias.bicep' = {
//@[00:265) └─DeclaredModuleExpression
//@[90:265)   ├─ObjectExpression
  name: 'moduleWithExtsWithoutAlaises'
//@[02:038)   | └─ObjectPropertyExpression
//@[02:006)   |   ├─StringLiteralExpression { Value = name }
//@[08:038)   |   └─StringLiteralExpression { Value = moduleWithExtsWithoutAlaises }
  extensionConfigs: {
//@[20:129)   └─ObjectExpression
    kubernetes: {
//@[04:101)     └─ObjectPropertyExpression
//@[04:014)       ├─StringLiteralExpression { Value = kubernetes }
//@[16:101)       └─ObjectExpression
      kubeConfig: 'kubeConfig2FromModule'
//@[06:041)         ├─ObjectPropertyExpression
//@[06:016)         | ├─StringLiteralExpression { Value = kubeConfig }
//@[18:041)         | └─StringLiteralExpression { Value = kubeConfig2FromModule }
      namespace: 'ns2FromModule'
//@[06:032)         └─ObjectPropertyExpression
//@[06:015)           ├─StringLiteralExpression { Value = namespace }
//@[17:032)           └─StringLiteralExpression { Value = ns2FromModule }
    }
  }
}

// END: Extension configs for modules

