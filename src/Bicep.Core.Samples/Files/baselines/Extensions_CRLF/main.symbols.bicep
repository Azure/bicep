// BEGIN: Extension declarations

// extension kubernetes with {
//   kubeConfig: 'DELETE'
//   namespace: 'DELETE'
// } as k8s

//extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0:0.1.8-preview' as graph

// END: Extension declarations

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
  name: 'moduleWithExtsWithoutAlaises'
  extensionConfigs: {
    kubernetes: {
      kubeConfig: 'kubeConfig2FromModule'
      namespace: 'ns2FromModule'
    }
  }
}

// END: Extension configs for modules

