// BEGIN: Parameters

param strParam1 string

@secure()
param secureStrParam1 string

param boolParam1 bool

// END: Parameters

// BEGIN: Variables

var strVar1 = 'strVar1Value'
var strParamVar1 = strParam1

// END: Variables

// BEGIN: Extension declarations

extension az
extension kubernetes  as k8s
extension 'br:mcr.microsoft.com/bicep/extensions/hasoptionalconfig/v1:1.2.3'  as extWithOptionalConfig1
extension 'br:mcr.microsoft.com/bicep/extensions/hasoptionalconfig/v1:1.2.3'  as extWithOptionalConfig2
extension 'br:mcr.microsoft.com/bicep/extensions/hasoptionalconfig/v1:1.2.3' with {
  optionalString: strParam1
} as extWithOptionalConfig3
extension 'br:mcr.microsoft.com/bicep/extensions/hassecureconfig/v1:1.2.3' with {
  requiredSecureString: secureStrParam1
} as extWithSecureStr1
extension 'br:mcr.microsoft.com/bicep/extensions/hasconfig/v1:1.2.3' with {
  requiredString: testResource1.id
} as extWithConfig1
extension 'br:mcr.microsoft.com/bicep/extensions/hasconfig/v1:1.2.3' with {
  requiredString: boolParam1 ? strParamVar1 : strParam1
} as extWithConfig2

// END: Extension declarations

// BEGIN: Key vaults

resource kv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'kv1'
}

resource scopedKv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'scopedKv1'
  scope: az.resourceGroup('otherGroup')
}

// END: Key vaults

// BEGIN: Test resources

resource testResource1 'az:My.Rp/TestType@2020-01-01' = {
  name: 'testResource1'
  properties: {}
}

resource aks 'Microsoft.ContainerService/managedClusters@2024-02-01' = {
  name: 'aksCluster'
  location: az.resourceGroup().location
}

// END: Test resources

// BEGIN: Extension configs for modules

module moduleWithExtsWithAliases 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  extensionConfigs: {
    k8s: {
      kubeConfig: 'kubeConfig2'
      namespace: 'ns2'
    }
  }
}

module moduleWithExtsWithoutAliases 'child/hasConfigurableExtensionsWithoutAlias.bicep' = {
  extensionConfigs: {
    kubernetes: {
      kubeConfig: 'kubeConfig2'
    }
  }
}

module moduleExtConfigsFromParams 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  extensionConfigs: {
    k8s: {
      kubeConfig: boolParam1 ? secureStrParam1 : strParam1
      namespace: boolParam1 ? strParam1 : 'falseCond'
    }
  }
}

module moduleExtConfigFromKeyVaultReference 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  extensionConfigs: {
    k8s: {
      kubeConfig: kv1.getSecret('myKubeConfig')
      namespace: strVar1
    }
  }
}

module moduleExtConfigFromReferences 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  extensionConfigs: {
    k8s: {
      kubeConfig: aks.listClusterAdminCredential().kubeconfigs[0].value
      namespace: testResource1.properties.namespace
    }
  }
}

module moduleWithExtsUsingFullInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  extensionConfigs: {
    k8s: k8s.config
  }
}

module moduleWithExtsUsingFullInheritanceTernary1 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  extensionConfigs: {
    k8s: k8s.config
    extWithOptionalConfig: boolParam1 ? extWithOptionalConfig1.config : extWithOptionalConfig2.config
  }
}

module moduleWithExtsUsingPiecemealInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  extensionConfigs: {
    k8s: {
      kubeConfig: k8s.config.kubeConfig
      namespace: k8s.config.namespace
    }
  }
}

module moduleWithExtsUsingPiecemealInheritanceLooped 'child/hasConfigurableExtensionsWithAlias.bicep' = [
  for i in range(0, 4): {
    name: 'moduleWithExtsPiecemealInheritanceLooped${i}'
    extensionConfigs: {
      k8s: {
        kubeConfig: k8s.config.kubeConfig
        namespace: k8s.config.namespace
      }
    }
  }
]

module moduleExtConfigsConditionalMixed 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  extensionConfigs: {
    k8s: {
      kubeConfig: boolParam1 ? secureStrParam1 : k8s.config.kubeConfig
      namespace: boolParam1 ? az.resourceGroup().location : k8s.config.namespace
    }
  }
}

module moduleWithExtsEmpty 'child/hasConfigurableExtensionsWithAlias.bicep' = {
  extensionConfigs: {
    k8s: k8s.config
    extWithOptionalConfig: {}
  }
}

// END: Extension configs for modules
