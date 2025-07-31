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
  extensionConfigs: {
    k8s: k8s // must use k8s.config
  }
}

module moduleInvalidPropertyAccess 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  extensionConfigs: {
    k8s: {
      kubeConfig: k8s.config.kubeConfig.keyVaultReference
      namespace: k8s.config.namespace.value
    }
  }
}

module moduleComplexKeyVaultReference 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  extensionConfigs: {
    k8s: {
      kubeConfig: boolParam1 ? kv1.getSecret('myKubeConfig') : scopedKv1.getSecret('myOtherKubeConfig')
      namespace: boolParam1 ? kv1.getSecret('myKubeConfig') : kv1.getSecret('myOtherKubeConfig')
    }
  }
}

var invalidVarAssignment1 = k8s.config.namespace
var invalidVarAssignment2 = k8s.config.kubeConfig

var extensionConfigsVar = {
  k8s: {
    kubeConfig: 'inlined',
    namespace: 'inlined'
  }
}

var k8sConfigDeployTime = {
  kubeConfig: strParam1
  namespace: strParam1
}

module moduleWithExtsUsingVar1 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  extensionConfigs: extensionConfigsVar
}

module moduleWithExtsUsingVar2 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  extensionConfigs: {
    k8s: k8sConfigDeployTime
  }
}

module moduleInvalidSpread1 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  extensionConfigs: {
    ...extensionConfigsVar
  }
}

module moduleInvalidSpread2 'child/hasConfigurableExtensionsWithAlias.bicep' = {
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

output k8sConfig object = k8s.config

output k8sNamespace string = k8s.config.namespace

// END: Outputs
