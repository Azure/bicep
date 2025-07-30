// BEGIN: Parameters

param boolParam1 bool
//@[06:16) Parameter boolParam1. Type: bool. Declaration start char: 0, length: 21
param strParam1 string
//@[06:15) Parameter strParam1. Type: string. Declaration start char: 0, length: 22

// END: Parameters

// BEGIN: Valid Extension declarations

extension az
//@[10:12) ImportedNamespace az. Type: az. Declaration start char: 0, length: 12
extension kubernetes as k8s
//@[24:27) ImportedNamespace k8s. Type: k8s. Declaration start char: 0, length: 27

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
//@[07:41) Module moduleWithExtsUsingFullInheritance. Type: module. Declaration start char: 0, length: 162
  extensionConfigs: {
    k8s: k8s // must use k8s.config
  }
}

module moduleInvalidPropertyAccess 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[07:34) Module moduleInvalidPropertyAccess. Type: module. Declaration start char: 0, length: 241
  extensionConfigs: {
    k8s: {
      kubeConfig: k8s.config.kubeConfig.keyVaultReference
      namespace: k8s.config.namespace.value
    }
  }
}

module moduleComplexKeyVaultReference 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[07:37) Module moduleComplexKeyVaultReference. Type: module. Declaration start char: 0, length: 343
  extensionConfigs: {
    k8s: {
      kubeConfig: boolParam1 ? kv1.getSecret('myKubeConfig') : scopedKv1.getSecret('myOtherKubeConfig')
      namespace: boolParam1 ? kv1.getSecret('myKubeConfig') : kv1.getSecret('myOtherKubeConfig')
    }
  }
}

var invalidVarAssignment1 = k8s.config.namespace
//@[04:25) Variable invalidVarAssignment1. Type: string. Declaration start char: 0, length: 48
var invalidVarAssignment2 = k8s.config.kubeConfig
//@[04:25) Variable invalidVarAssignment2. Type: string. Declaration start char: 0, length: 49

var extensionConfigsVar = {
//@[04:23) Variable extensionConfigsVar. Type: object. Declaration start char: 0, length: 99
  k8s: {
    kubeConfig: 'inlined',
    namespace: 'inlined'
  }
}

var k8sConfigDeployTime = {
//@[04:23) Variable k8sConfigDeployTime. Type: object. Declaration start char: 0, length: 79
  kubeConfig: strParam1
  namespace: strParam1
}

module moduleWithExtsUsingVar1 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[07:30) Module moduleWithExtsUsingVar1. Type: module. Declaration start char: 0, length: 127
  extensionConfigs: extensionConfigsVar
}

module moduleWithExtsUsingVar2 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[07:30) Module moduleWithExtsUsingVar2. Type: module. Declaration start char: 0, length: 144
  extensionConfigs: {
    k8s: k8sConfigDeployTime
  }
}

module moduleInvalidSpread1 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[07:27) Module moduleInvalidSpread1. Type: module. Declaration start char: 0, length: 139
  extensionConfigs: {
    ...extensionConfigsVar
  }
}

module moduleInvalidSpread2 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[07:27) Module moduleInvalidSpread2. Type: module. Declaration start char: 0, length: 160
  extensionConfigs: {
    k8s: {
      ...k8sConfigDeployTime
    }
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

