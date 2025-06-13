// BEGIN: Parameters

param boolParam1 bool
//@[06:16) Parameter boolParam1. Type: bool. Declaration start char: 0, length: 21

// END: Parameters

// BEGIN: Valid Extension declarations

extension az
//@[10:12) ImportedNamespace az. Type: az. Declaration start char: 0, length: 12
extension kubernetes with {
  kubeConfig: 'DELETE'
  namespace: 'DELETE'
} as k8s
//@[05:08) ImportedNamespace k8s. Type: k8s. Declaration start char: 0, length: 84

//extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0:0.1.8-preview' as graph

// END: Valid Extension declarations

// BEGIN: Key vaults

resource kv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[09:12) Resource kv1. Type: Microsoft.KeyVault/vaults@2019-09-01. Declaration start char: 0, length: 82
  name: 'kv1'
}

resource scopedKv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[09:18) Resource scopedKv1. Type: Microsoft.KeyVault/vaults@2019-09-01. Declaration start char: 0, length: 132
  name: 'scopedKv1'
  scope: resourceGroup('otherGroup')
}

// END: Key Vaults

// BEGIN: Resources

var configProp = 'config'
//@[04:14) Variable configProp. Type: 'config'. Declaration start char: 0, length: 25

// Extension symbols are blocked in resources because each config property returns an object { value, keyVaultReference } and "value" is not available when a reference is provided.
// Users should use deployment parameters for this scenario.
resource testResource1 'az:My.Rp/TestType@2020-01-01' = {
//@[09:22) Resource testResource1. Type: My.Rp/TestType@2020-01-01. Declaration start char: 0, length: 231
  name: k8s.config.namespace
  properties: {
    secret: k8s.config.kubeConfig
    ns: k8s[configProp].namespace
    ref: k8s[kv1.properties.sku.name].namespace
  }
}

// END: Resources

// BEGIN: Extension configs for modules

module moduleWithExtsUsingFullInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[07:41) Module moduleWithExtsUsingFullInheritance. Type: module. Declaration start char: 0, length: 203
  name: 'moduleWithExtsFullInheritance'
  extensionConfigs: {
    k8s: k8s // must use k8s.config
  }
}

module moduleInvalidPropertyAccess 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[07:34) Module moduleInvalidPropertyAccess. Type: module. Declaration start char: 0, length: 280
  name: 'moduleInvalidPropertyAccess'
  extensionConfigs: {
    k8s: {
      kubeConfig: k8s.config.kubeConfig.keyVaultReference
      namespace: k8s.config.namespace.value
    }
  }
}

module moduleComplexKeyVaultReference 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[07:37) Module moduleComplexKeyVaultReference. Type: module. Declaration start char: 0, length: 385
  name: 'moduleComplexKeyVaultReference'
  extensionConfigs: {
    k8s: {
      kubeConfig: boolParam1 ? kv1.getSecret('myKubeConfig') : scopedKv1.getSecret('myOtherKubeConfig')
      namespace: boolParam1 ? kv1.getSecret('myKubeConfig') : kv1.getSecret('myOtherKubeConfig')
    }
  }
}

// TODO(kylealbert): Figure out if this can be made allowable easily, potentially by inlining.
var k8sConfigDeployTime = {
//@[04:23) Variable k8sConfigDeployTime. Type: error. Declaration start char: 0, length: 91
  kubeConfig: k8s.config.kubeConfig
  namespace: strParam1
}

module moduleWithExtsUsingVar 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[07:29) Module moduleWithExtsUsingVar. Type: module. Declaration start char: 0, length: 177
  name: 'moduleWithExtsUsingVar'
  extensionConfigs: {
    k8s: k8sConfigDeployTime
  }
}

// END: Extension configs for modules

// BEGIN: Outputs

// Extension symbols are blocked for outputs for now. Users should use deployment parameters for this scenario.

output k8sTheNamespace object = k8s // This is a namespace type
//@[07:22) Output k8sTheNamespace. Type: object. Declaration start char: 0, length: 35

output k8sConfig object = k8s.config
//@[07:16) Output k8sConfig. Type: object. Declaration start char: 0, length: 36

output k8sNamespace string = k8s.config.namespace
//@[07:19) Output k8sNamespace. Type: string. Declaration start char: 0, length: 49

// END: Outputs

