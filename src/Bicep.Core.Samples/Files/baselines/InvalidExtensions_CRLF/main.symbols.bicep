// BEGIN: Valid Extension declarations

extension kubernetes with {
  kubeConfig: 'DELETE'
  namespace: 'DELETE'
} as k8s
//@[5:08) ImportedNamespace k8s. Type: k8s. Declaration start char: 0, length: 84

//extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0:0.1.8-preview' as graph

// END: Valid Extension declarations

// BEGIN: Extension configs for modules

module moduleWithExtsUsingFullInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[7:41) Module moduleWithExtsUsingFullInheritance. Type: module. Declaration start char: 0, length: 203
  name: 'moduleWithExtsFullInheritance'
  extensionConfigs: {
    k8s: k8s // must use k8s.config
  }
}

// END: Extension configs for modules

