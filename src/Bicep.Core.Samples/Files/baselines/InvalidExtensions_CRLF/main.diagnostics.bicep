// BEGIN: Parameters

param boolParam1 bool
param strParam1 string

// END: Parameters

// BEGIN: Valid Extension declarations

extension az
extension kubernetes as k8s

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
//@[23:053) [BCP081 (Warning)] Resource type "My.Rp/TestType@2020-01-01" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed. (bicep https://aka.ms/bicep/core-diagnostics#BCP081) |'az:My.Rp/TestType@2020-01-01'|
  name: k8s.config.namespace
//@[08:011) [BCP418 (Error)] Extensions cannot be referenced here. Extensions can only be referenced by module extension configurations. (bicep https://aka.ms/bicep/core-diagnostics#BCP418) |k8s|
  properties: {
    secret: k8s.config.kubeConfig
//@[12:015) [BCP418 (Error)] Extensions cannot be referenced here. Extensions can only be referenced by module extension configurations. (bicep https://aka.ms/bicep/core-diagnostics#BCP418) |k8s|
    ns: k8s[configProp].namespace
//@[08:011) [BCP418 (Error)] Extensions cannot be referenced here. Extensions can only be referenced by module extension configurations. (bicep https://aka.ms/bicep/core-diagnostics#BCP418) |k8s|
    ref: k8s[kv1.properties.sku.name].namespace
//@[09:012) [BCP418 (Error)] Extensions cannot be referenced here. Extensions can only be referenced by module extension configurations. (bicep https://aka.ms/bicep/core-diagnostics#BCP418) |k8s|
  }
}

// END: Resources

// BEGIN: Extension configs for modules

module moduleWithExtsUsingFullInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  extensionConfigs: {
    k8s: k8s // must use k8s.config
//@[09:012) [BCP036 (Error)] The property "k8s" expected a value of type "configuration" but the provided value is of type "k8s". (bicep https://aka.ms/bicep/core-diagnostics#BCP036) |k8s|
  }
}

module moduleInvalidPropertyAccess 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  extensionConfigs: {
    k8s: {
      kubeConfig: k8s.config.kubeConfig.keyVaultReference
//@[18:057) [stacks-extensibility-compat (Info)] Secure config property values must be a key vault reference to be valid for Deployment stack deployments. (bicep core linter https://aka.ms/bicep/linter-diagnostics#stacks-extensibility-compat) |k8s.config.kubeConfig.keyVaultReference|
//@[40:057) [BCP055 (Error)] Cannot access properties of type "string". An "object" type is required. (bicep https://aka.ms/bicep/core-diagnostics#BCP055) |keyVaultReference|
      namespace: k8s.config.namespace.value
//@[38:043) [BCP055 (Error)] Cannot access properties of type "string". An "object" type is required. (bicep https://aka.ms/bicep/core-diagnostics#BCP055) |value|
    }
  }
}

module moduleComplexKeyVaultReference 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  extensionConfigs: {
    k8s: {
      kubeConfig: boolParam1 ? kv1.getSecret('myKubeConfig') : scopedKv1.getSecret('myOtherKubeConfig')
//@[18:103) [stacks-extensibility-compat (Info)] Secure config property values must be a key vault reference to be valid for Deployment stack deployments. (bicep core linter https://aka.ms/bicep/linter-diagnostics#stacks-extensibility-compat) |boolParam1 ? kv1.getSecret('myKubeConfig') : scopedKv1.getSecret('myOtherKubeConfig')|
      namespace: boolParam1 ? kv1.getSecret('myKubeConfig') : kv1.getSecret('myOtherKubeConfig')
//@[30:059) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator or a secure extension configuration property. (bicep https://aka.ms/bicep/core-diagnostics#BCP180) |kv1.getSecret('myKubeConfig')|
//@[62:096) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator or a secure extension configuration property. (bicep https://aka.ms/bicep/core-diagnostics#BCP180) |kv1.getSecret('myOtherKubeConfig')|
    }
  }
}

var invalidVarAssignment1 = k8s.config.namespace
//@[04:025) [no-unused-vars (Warning)] Variable "invalidVarAssignment1" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |invalidVarAssignment1|
//@[28:031) [BCP418 (Error)] Extensions cannot be referenced here. Extensions can only be referenced by module extension configurations. (bicep https://aka.ms/bicep/core-diagnostics#BCP418) |k8s|
var invalidVarAssignment2 = k8s.config.kubeConfig
//@[04:025) [no-unused-vars (Warning)] Variable "invalidVarAssignment2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |invalidVarAssignment2|
//@[28:031) [BCP418 (Error)] Extensions cannot be referenced here. Extensions can only be referenced by module extension configurations. (bicep https://aka.ms/bicep/core-diagnostics#BCP418) |k8s|

var extensionConfigsVar = {
  k8s: {
    kubeConfig: 'inlined',
//@[26:026) [BCP238 (Error)] Unexpected new line character after a comma. (bicep https://aka.ms/bicep/core-diagnostics#BCP238) ||
    namespace: 'inlined'
  }
}

var k8sConfigDeployTime = {
  kubeConfig: strParam1
  namespace: strParam1
}

module moduleWithExtsUsingVar1 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  extensionConfigs: extensionConfigsVar
//@[20:039) [BCP183 (Error)] The value of the module "extensionConfigs" property must be an object literal. (bicep https://aka.ms/bicep/core-diagnostics#BCP183) |extensionConfigsVar|
}

module moduleWithExtsUsingVar2 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  extensionConfigs: {
    k8s: k8sConfigDeployTime
  }
}

module moduleInvalidSpread1 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  extensionConfigs: {
    ...extensionConfigsVar
//@[04:026) [BCP401 (Error)] The spread operator "..." is not permitted in this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP401) |...extensionConfigsVar|
  }
}

module moduleInvalidSpread2 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  extensionConfigs: {
    k8s: {
      ...k8sConfigDeployTime
//@[06:028) [BCP401 (Error)] The spread operator "..." is not permitted in this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP401) |...k8sConfigDeployTime|
    }
  }
}

// END: Extension configs for modules

// BEGIN: Outputs

// Extension symbols are blocked for outputs for now. Users should use deployment parameters for this scenario.

output k8sTheNamespace object = k8s // This is a namespace type
//@[23:029) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |object|
//@[32:035) [BCP033 (Error)] Expected a value of type "object" but the provided value is of type "k8s". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |k8s|
//@[32:035) [BCP418 (Error)] Extensions cannot be referenced here. Extensions can only be referenced by module extension configurations. (bicep https://aka.ms/bicep/core-diagnostics#BCP418) |k8s|
//@[32:035) [outputs-should-not-contain-secrets (Warning)] Outputs should not contain secrets. Found possible secret: secure value 'k8s.config.kubeConfig' (bicep core linter https://aka.ms/bicep/linter-diagnostics#outputs-should-not-contain-secrets) |k8s|

output k8sConfig object = k8s.config
//@[17:023) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |object|
//@[26:029) [BCP418 (Error)] Extensions cannot be referenced here. Extensions can only be referenced by module extension configurations. (bicep https://aka.ms/bicep/core-diagnostics#BCP418) |k8s|
//@[30:036) [outputs-should-not-contain-secrets (Warning)] Outputs should not contain secrets. Found possible secret: secure value 'k8s.config.kubeConfig' (bicep core linter https://aka.ms/bicep/linter-diagnostics#outputs-should-not-contain-secrets) |config|

output k8sNamespace string = k8s.config.namespace
//@[29:032) [BCP418 (Error)] Extensions cannot be referenced here. Extensions can only be referenced by module extension configurations. (bicep https://aka.ms/bicep/core-diagnostics#BCP418) |k8s|

// END: Outputs

