// BEGIN: Parameters

param boolParam1 bool

// END: Parameters

// BEGIN: Valid Extension declarations

extension kubernetes with {
  kubeConfig: 'DELETE'
  namespace: 'DELETE'
} as k8s

//extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0:0.1.8-preview' as graph

// END: Valid Extension declarations

// BEGIN: Key vaults

resource kv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'kv1'
}

resource scopedKv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'scopedKv1'
  scope: resourceGroup('otherGroup')
}

// END: Key Vaults

// BEGIN: Extension configs for modules

module moduleWithExtsUsingFullInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  name: 'moduleWithExtsFullInheritance'
  extensionConfigs: {
    k8s: k8s // must use k8s.config
//@[09:12) [BCP036 (Error)] The property "k8s" expected a value of type "configuration" but the provided value is of type "k8s". (bicep https://aka.ms/bicep/core-diagnostics#BCP036) |k8s|
  }
}

module moduleInvalidPropertyAccess 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  name: 'moduleInvalidPropertyAccess'
  extensionConfigs: {
    k8s: {
      kubeConfig: k8s.config.kubeConfig.keyVaultReference
//@[40:57) [BCP055 (Error)] Cannot access properties of type "string". An "object" type is required. (bicep https://aka.ms/bicep/core-diagnostics#BCP055) |keyVaultReference|
      namespace: k8s.config.namespace.value
//@[38:43) [BCP055 (Error)] Cannot access properties of type "string". An "object" type is required. (bicep https://aka.ms/bicep/core-diagnostics#BCP055) |value|
    }
  }
}

module moduleComplexKeyVaultReference 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  name: 'moduleComplexKeyVaultReference'
  extensionConfigs: {
    k8s: {
      kubeConfig: boolParam1 ? kv1.getSecret('myKubeConfig') : scopedKv1.getSecret('myOtherKubeConfig')
      namespace: boolParam1 ? kv1.getSecret('myKubeConfig') : kv1.getSecret('myOtherKubeConfig')
//@[30:59) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP180) |kv1.getSecret('myKubeConfig')|
//@[62:96) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP180) |kv1.getSecret('myOtherKubeConfig')|
    }
  }
}

// END: Extension configs for modules

// BEGIN: Outputs

output k8sNamespace object = k8s // This is a namespace type
//@[20:26) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter/use-user-defined-types) |object|
//@[29:32) [BCP033 (Error)] Expected a value of type "object" but the provided value is of type "k8s". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |k8s|
//@[29:32) [outputs-should-not-contain-secrets (Warning)] Outputs should not contain secrets. Found possible secret: secure value 'k8s.config.kubeConfig' (bicep core linter https://aka.ms/bicep/linter/outputs-should-not-contain-secrets) |k8s|

// END: Outputs

