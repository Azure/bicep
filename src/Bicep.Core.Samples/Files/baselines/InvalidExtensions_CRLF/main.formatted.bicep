// BEGIN: Parameters

param boolParam1 bool

// END: Parameters

// BEGIN: Valid Extension declarations

extension az
extension kubernetes  as k8s

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
  name: 'moduleWithExtsFullInheritance'
  extensionConfigs: {
    k8s: k8s // must use k8s.config
  }
}

module moduleInvalidPropertyAccess 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  name: 'moduleInvalidPropertyAccess'
  extensionConfigs: {
    k8s: {
      kubeConfig: k8s.config.kubeConfig.keyVaultReference
      namespace: k8s.config.namespace.value
    }
  }
}

module moduleComplexKeyVaultReference 'child/hasConfigurableExtensionsWithAlias.bicep' = {
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
  kubeConfig: k8s.config.kubeConfig
  namespace: strParam1
}

module moduleWithExtsUsingVar 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  name: 'moduleWithExtsUsingVar'
  extensionConfigs: {
    k8s: k8sConfigDeployTime
  }
}

// END: Extension configs for modules

// BEGIN: Outputs

// Extension symbols are blocked for outputs for now. Users should use deployment parameters for this scenario.

output k8sTheNamespace object = k8s // This is a namespace type

output k8sConfig object = k8s.config

output k8sNamespace string = k8s.config.namespace

// END: Outputs
