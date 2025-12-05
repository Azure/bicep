// BEGIN: Parameters

param boolParam1 bool
//@[06:016) Parameter boolParam1. Type: bool. Declaration start char: 0, length: 21
param strParam1 string
//@[06:015) Parameter strParam1. Type: string. Declaration start char: 0, length: 22
param objParam1 object
//@[06:015) Parameter objParam1. Type: object. Declaration start char: 0, length: 22
param invalidParamAssignment1 string = k8s.config.namespace
//@[06:029) Parameter invalidParamAssignment1. Type: string. Declaration start char: 0, length: 59

// END: Parameters

// BEGIN: Valid extension declarations

extension az
//@[10:012) ImportedNamespace az. Type: az. Declaration start char: 0, length: 12
extension kubernetes as k8s
//@[24:027) ImportedNamespace k8s. Type: k8s. Declaration start char: 0, length: 27
extension 'br:mcr.microsoft.com/bicep/extensions/hasoptionalconfig/v1:1.2.3' as extWithOptionalConfig1
//@[80:102) ImportedNamespace extWithOptionalConfig1. Type: extWithOptionalConfig1. Declaration start char: 0, length: 102

// END: Valid extension declarations

// BEGIN: Invalid extension declarations

extension 'br:mcr.microsoft.com/bicep/extensions/hasoptionalconfig/v1:1.2.3' with {
  optionalString: testResource1.properties.ns // no reference calls, use module extension configs instead.
} as invalidExtDecl1
//@[05:020) ImportedNamespace invalidExtDecl1. Type: invalidExtDecl1. Declaration start char: 0, length: 213

extension 'br:mcr.microsoft.com/bicep/extensions/hasoptionalconfig/v1:1.2.3' with {
  optionalString: newGuid()
} as invalidExtDecl2
//@[05:020) ImportedNamespace invalidExtDecl2. Type: invalidExtDecl2. Declaration start char: 0, length: 134

extension 'br:mcr.microsoft.com/bicep/extensions/hassecureconfig/v1:1.2.3' with {
  requiredSecureString: kv1.getSecret('abc')
} as invalidExtDecl3
//@[05:020) ImportedNamespace invalidExtDecl3. Type: invalidExtDecl3. Declaration start char: 0, length: 149

// END: Invalid extension declarations

// BEGIN: Key vaults

resource kv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[09:012) Resource kv1. Type: Microsoft.KeyVault/vaults@2019-09-01. Declaration start char: 0, length: 82
  name: 'kv1'
}

resource scopedKv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[09:018) Resource scopedKv1. Type: Microsoft.KeyVault/vaults@2019-09-01. Declaration start char: 0, length: 132
  name: 'scopedKv1'
  scope: resourceGroup('otherGroup')
}

// END: Key Vaults

// BEGIN: Resources

var configProp = 'config'
//@[04:014) Variable configProp. Type: 'config'. Declaration start char: 0, length: 25

// Extension symbols are blocked in resources because each config property returns an object { value, keyVaultReference } and "value" is not available when a reference is provided.
// Users should use deployment parameters for this scenario.
resource testResource1 'az:My.Rp/TestType@2020-01-01' = {
//@[09:022) Resource testResource1. Type: My.Rp/TestType@2020-01-01. Declaration start char: 0, length: 231
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
//@[07:041) Module moduleWithExtsUsingFullInheritance. Type: module. Declaration start char: 0, length: 162
  extensionConfigs: {
    k8s: k8s // must use k8s.config
  }
}

module moduleInvalidPropertyAccess 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[07:034) Module moduleInvalidPropertyAccess. Type: module. Declaration start char: 0, length: 241
  extensionConfigs: {
    k8s: {
      kubeConfig: k8s.config.kubeConfig.keyVaultReference
      namespace: k8s.config.namespace.value
    }
  }
}

module moduleComplexKeyVaultReference 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[07:037) Module moduleComplexKeyVaultReference. Type: module. Declaration start char: 0, length: 343
  extensionConfigs: {
    k8s: {
      kubeConfig: boolParam1 ? kv1.getSecret('myKubeConfig') : scopedKv1.getSecret('myOtherKubeConfig')
      namespace: boolParam1 ? kv1.getSecret('myKubeConfig') : kv1.getSecret('myOtherKubeConfig')
    }
  }
}

var invalidVarAssignment1 = k8s.config.namespace
//@[04:025) Variable invalidVarAssignment1. Type: string. Declaration start char: 0, length: 48
var invalidVarAssignment2 = k8s.config.kubeConfig
//@[04:025) Variable invalidVarAssignment2. Type: string. Declaration start char: 0, length: 49

var extensionConfigsVar = {
//@[04:023) Variable extensionConfigsVar. Type: object. Declaration start char: 0, length: 99
  k8s: {
    kubeConfig: 'inlined',
    namespace: 'inlined'
  }
}

var k8sConfigDeployTime = {
//@[04:023) Variable k8sConfigDeployTime. Type: object. Declaration start char: 0, length: 79
  kubeConfig: strParam1
  namespace: strParam1
}

module moduleWithExtsUsingVar1 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[07:030) Module moduleWithExtsUsingVar1. Type: module. Declaration start char: 0, length: 127
  extensionConfigs: extensionConfigsVar
}

module moduleWithExtsUsingVar2 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[07:030) Module moduleWithExtsUsingVar2. Type: module. Declaration start char: 0, length: 144
  extensionConfigs: {
    k8s: k8sConfigDeployTime
  }
}

module moduleWithExtsUsingParam1 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[07:032) Module moduleWithExtsUsingParam1. Type: module. Declaration start char: 0, length: 136
  extensionConfigs: {
    k8s: objParam1
  }
}

module moduleWithExtsUsingReference1 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[07:036) Module moduleWithExtsUsingReference1. Type: module. Declaration start char: 0, length: 155
  extensionConfigs: {
    k8s: testResource1.properties
  }
}

module moduleInvalidSpread1 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[07:027) Module moduleInvalidSpread1. Type: module. Declaration start char: 0, length: 139
  extensionConfigs: {
    ...extensionConfigsVar
  }
}

module moduleInvalidSpread2 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[07:027) Module moduleInvalidSpread2. Type: module. Declaration start char: 0, length: 160
  extensionConfigs: {
    k8s: {
      ...k8sConfigDeployTime
    }
  }
}

module moduleInvalidInheritanceTernary1 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[07:039) Module moduleInvalidInheritanceTernary1. Type: module. Declaration start char: 0, length: 229
  extensionConfigs: {
    k8s: k8s.config
    extWithOptionalConfig1: boolParam1 ? extWithOptionalConfig1.config : k8s.config
  }
}

module moduleInvalidInheritanceTernary2 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[07:039) Module moduleInvalidInheritanceTernary2. Type: module. Declaration start char: 0, length: 339
  extensionConfigs: {
    k8s: k8s.config
    extWithOptionalConfig1: boolParam1 ? extWithOptionalConfig1.config : { optionalString: 'value' } // limitation: cannot mix these currently due to special code gen needed for object literals
  }
}

// END: Extension configs for modules

// BEGIN: Outputs

// Extension symbols are blocked for outputs for now. Users should use deployment parameters for this scenario.

output k8sTheNamespace object = k8s // This is a namespace type
//@[07:022) Output k8sTheNamespace. Type: object. Declaration start char: 0, length: 35

output k8sConfig object = k8s.config
//@[07:016) Output k8sConfig. Type: object. Declaration start char: 0, length: 36

output k8sNamespace string = k8s.config.namespace
//@[07:019) Output k8sNamespace. Type: string. Declaration start char: 0, length: 49

// END: Outputs

