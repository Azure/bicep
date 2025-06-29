// BEGIN: Parameters

param boolParam1 bool

// END: Parameters

// BEGIN: Valid Extension declarations

extension az
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

// BEGIN: Resources

var configProp = 'config'

// Extension symbols are blocked in resources because each config property returns an object { value, keyVaultReference } and "value" is not available when a reference is provided.
// Users should use deployment parameters for this scenario.
resource testResource1 'az:My.Rp/TestType@2020-01-01' = {
//@[23:53) [BCP081 (Warning)] Resource type "My.Rp/TestType@2020-01-01" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed. (bicep https://aka.ms/bicep/core-diagnostics#BCP081) |'az:My.Rp/TestType@2020-01-01'|
  name: k8s.config.namespace
//@[08:11) [BCP418 (Error)] Extensions cannot be referenced here. Extensions can only be referenced by module extension configurations. (bicep https://aka.ms/bicep/core-diagnostics#BCP418) |k8s|
  properties: {
    secret: k8s.config.kubeConfig
//@[12:15) [BCP418 (Error)] Extensions cannot be referenced here. Extensions can only be referenced by module extension configurations. (bicep https://aka.ms/bicep/core-diagnostics#BCP418) |k8s|
    ns: k8s[configProp].namespace
//@[08:11) [BCP418 (Error)] Extensions cannot be referenced here. Extensions can only be referenced by module extension configurations. (bicep https://aka.ms/bicep/core-diagnostics#BCP418) |k8s|
    ref: k8s[kv1.properties.sku.name].namespace
//@[09:12) [BCP418 (Error)] Extensions cannot be referenced here. Extensions can only be referenced by module extension configurations. (bicep https://aka.ms/bicep/core-diagnostics#BCP418) |k8s|
  }
}

// END: Resources

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
//@[30:59) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator or a secure extension configuration property. (bicep https://aka.ms/bicep/core-diagnostics#BCP180) |kv1.getSecret('myKubeConfig')|
//@[62:96) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator or a secure extension configuration property. (bicep https://aka.ms/bicep/core-diagnostics#BCP180) |kv1.getSecret('myOtherKubeConfig')|
    }
  }
}

// TODO(kylealbert): Figure out if this can be made allowable easily, potentially by inlining.
var k8sConfigDeployTime = {
  kubeConfig: k8s.config.kubeConfig
//@[14:17) [BCP418 (Error)] Extensions cannot be referenced here. Extensions can only be referenced by module extension configurations. (bicep https://aka.ms/bicep/core-diagnostics#BCP418) |k8s|
  namespace: strParam1
//@[13:22) [BCP057 (Error)] The name "strParam1" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |strParam1|
}

module moduleWithExtsUsingVar 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  name: 'moduleWithExtsUsingVar'
  extensionConfigs: {
    k8s: k8sConfigDeployTime
//@[09:28) [BCP062 (Error)] The referenced declaration with name "k8sConfigDeployTime" is not valid. (bicep https://aka.ms/bicep/core-diagnostics#BCP062) |k8sConfigDeployTime|
  }
}

// END: Extension configs for modules

// BEGIN: Outputs

// Extension symbols are blocked for outputs for now. Users should use deployment parameters for this scenario.

output k8sTheNamespace object = k8s // This is a namespace type
//@[23:29) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |object|
//@[32:35) [BCP033 (Error)] Expected a value of type "object" but the provided value is of type "k8s". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |k8s|
//@[32:35) [BCP418 (Error)] Extensions cannot be referenced here. Extensions can only be referenced by module extension configurations. (bicep https://aka.ms/bicep/core-diagnostics#BCP418) |k8s|
//@[32:35) [outputs-should-not-contain-secrets (Warning)] Outputs should not contain secrets. Found possible secret: secure value 'k8s.config.kubeConfig' (bicep core linter https://aka.ms/bicep/linter-diagnostics#outputs-should-not-contain-secrets) |k8s|

output k8sConfig object = k8s.config
//@[17:23) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |object|
//@[26:29) [BCP418 (Error)] Extensions cannot be referenced here. Extensions can only be referenced by module extension configurations. (bicep https://aka.ms/bicep/core-diagnostics#BCP418) |k8s|
//@[30:36) [outputs-should-not-contain-secrets (Warning)] Outputs should not contain secrets. Found possible secret: secure value 'k8s.config.kubeConfig' (bicep core linter https://aka.ms/bicep/linter-diagnostics#outputs-should-not-contain-secrets) |config|

output k8sNamespace string = k8s.config.namespace
//@[29:32) [BCP418 (Error)] Extensions cannot be referenced here. Extensions can only be referenced by module extension configurations. (bicep https://aka.ms/bicep/core-diagnostics#BCP418) |k8s|

// END: Outputs

