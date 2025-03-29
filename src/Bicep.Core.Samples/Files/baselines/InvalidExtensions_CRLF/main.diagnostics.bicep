// BEGIN: Valid Extension declarations

extension kubernetes with {
  kubeConfig: 'DELETE'
  namespace: 'DELETE'
} as k8s

//extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0:0.1.8-preview' as graph

// END: Valid Extension declarations

// BEGIN: Extension configs for modules

module moduleWithExtsUsingFullInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  name: 'moduleWithExtsFullInheritance'
  extensionConfigs: {
    k8s: k8s // must use k8s.config
//@[9:12) [BCP036 (Error)] The property "k8s" expected a value of type "configuration" but the provided value is of type "k8s". (bicep https://aka.ms/bicep/core-diagnostics#BCP036) |k8s|
  }
}

// END: Extension configs for modules

