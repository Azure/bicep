// BEGIN: Extension declarations

// extension kubernetes with {
//   kubeConfig: 'DELETE'
//   namespace: 'DELETE'
// } as k8s

//extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0:0.1.8-preview' as graph

// END: Extension declarations

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
  name: 'moduleWithExtsWithoutAlaises'
  extensionConfigs: {
    kubernetes: {
      kubeConfig: 'kubeConfig2FromModule'
      namespace: 'ns2FromModule'
    }
  }
}

// END: Extension configs for modules

