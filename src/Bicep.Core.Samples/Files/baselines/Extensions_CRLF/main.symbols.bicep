// BEGIN: Parameters

param strParam1 string
//@[006:015) Parameter strParam1. Type: string. Declaration start char: 0, length: 22

@secure()
param secureStrParam1 string
//@[006:021) Parameter secureStrParam1. Type: string. Declaration start char: 0, length: 39

param boolParam1 bool
//@[006:016) Parameter boolParam1. Type: bool. Declaration start char: 0, length: 21

// END: Parameters

// BEGIN: Variables

var strVar1 = 'strVar1Value'
//@[004:011) Variable strVar1. Type: 'strVar1Value'. Declaration start char: 0, length: 28
var strParamVar1 = strParam1
//@[004:016) Variable strParamVar1. Type: string. Declaration start char: 0, length: 28

// END: Variables

// BEGIN: Extension declarations

extension az
//@[010:012) ImportedNamespace az. Type: az. Declaration start char: 0, length: 12
extension kubernetes as k8s
//@[024:027) ImportedNamespace k8s. Type: k8s. Declaration start char: 0, length: 27
extension 'br:mcr.microsoft.com/bicep/extensions/hasoptionalconfig/v1:1.2.3' as extWithOptionalConfig1
//@[080:102) ImportedNamespace extWithOptionalConfig1. Type: extWithOptionalConfig1. Declaration start char: 0, length: 102
extension 'br:mcr.microsoft.com/bicep/extensions/hasoptionalconfig/v1:1.2.3' as extWithOptionalConfig2
//@[080:102) ImportedNamespace extWithOptionalConfig2. Type: extWithOptionalConfig2. Declaration start char: 0, length: 102
extension 'br:mcr.microsoft.com/bicep/extensions/hasoptionalconfig/v1:1.2.3' with {
  optionalString: strParam1
} as extWithOptionalConfig3
//@[005:027) ImportedNamespace extWithOptionalConfig3. Type: extWithOptionalConfig3. Declaration start char: 0, length: 141
extension 'br:mcr.microsoft.com/bicep/extensions/hassecureconfig/v1:1.2.3' with {
  requiredSecureString: secureStrParam1
} as extWithSecureStr1
//@[005:022) ImportedNamespace extWithSecureStr1. Type: extWithSecureStr1. Declaration start char: 0, length: 146
extension 'br:mcr.microsoft.com/bicep/extensions/hasconfig/v1:1.2.3' with {
  requiredString: testResource1.id
} as extWithConfig1
//@[005:019) ImportedNamespace extWithConfig1. Type: extWithConfig1. Declaration start char: 0, length: 132
extension 'br:mcr.microsoft.com/bicep/extensions/hasconfig/v1:1.2.3' with {
  requiredString: boolParam1 ? strParamVar1 : strParam1
} as extWithConfig2
//@[005:019) ImportedNamespace extWithConfig2. Type: extWithConfig2. Declaration start char: 0, length: 153

// END: Extension declarations

// BEGIN: Key vaults

resource kv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[009:012) Resource kv1. Type: Microsoft.KeyVault/vaults@2019-09-01. Declaration start char: 0, length: 82
  name: 'kv1'
}

resource scopedKv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[009:018) Resource scopedKv1. Type: Microsoft.KeyVault/vaults@2019-09-01. Declaration start char: 0, length: 135
  name: 'scopedKv1'
  scope: az.resourceGroup('otherGroup')
}

// END: Key vaults

// BEGIN: Test resources

resource testResource1 'az:My.Rp/TestType@2020-01-01' = {
//@[009:022) Resource testResource1. Type: My.Rp/TestType@2020-01-01. Declaration start char: 0, length: 103
  name: 'testResource1'
  properties: {}
}

resource aks 'Microsoft.ContainerService/managedClusters@2024-02-01' = {
//@[009:012) Resource aks. Type: Microsoft.ContainerService/managedClusters@2024-02-01. Declaration start char: 0, length: 138
  name: 'aksCluster'
  location: az.resourceGroup().location
}

// END: Test resources

// BEGIN: Extension configs for modules

module moduleWithExtsWithAliases 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[007:032) Module moduleWithExtsWithAliases. Type: module. Declaration start char: 0, length: 192
  extensionConfigs: {
    k8s: {
      kubeConfig: 'kubeConfig2'
      namespace: 'ns2'
    }
  }
}

module moduleWithExtsWithoutAliases 'child/hasConfigurableExtensionsWithoutAlias.bicep' = {
//@[007:035) Module moduleWithExtsWithoutAliases. Type: module. Declaration start char: 0, length: 181
  extensionConfigs: {
    kubernetes: {
      kubeConfig: 'kubeConfig2'
    }
  }
}

module moduleExtConfigsFromParams 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[007:033) Module moduleExtConfigsFromParams. Type: module. Declaration start char: 0, length: 251
  extensionConfigs: {
    k8s: {
      kubeConfig: boolParam1 ? secureStrParam1 : strParam1
      namespace: boolParam1 ? strParam1 : 'falseCond'
    }
  }
}

module moduleExtConfigFromKeyVaultReference 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[007:043) Module moduleExtConfigFromKeyVaultReference. Type: module. Declaration start char: 0, length: 221
  extensionConfigs: {
    k8s: {
      kubeConfig: kv1.getSecret('myKubeConfig')
      namespace: strVar1
    }
  }
}

module moduleExtConfigFromReferences 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[007:036) Module moduleExtConfigFromReferences. Type: module. Declaration start char: 0, length: 265
  extensionConfigs: {
    k8s: {
      kubeConfig: aks.listClusterAdminCredential().kubeconfigs[0].value
      namespace: testResource1.properties.namespace
    }
  }
}

module moduleWithExtsUsingFullInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[007:041) Module moduleWithExtsUsingFullInheritance. Type: module. Declaration start char: 0, length: 146
  extensionConfigs: {
    k8s: k8s.config
  }
}

module moduleWithExtsUsingFullInheritanceTernary1 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[007:049) Module moduleWithExtsUsingFullInheritanceTernary1. Type: module. Declaration start char: 0, length: 257
  extensionConfigs: {
    k8s: k8s.config
    extWithOptionalConfig: boolParam1 ? extWithOptionalConfig1.config : extWithOptionalConfig2.config
  }
}

module moduleWithExtsUsingPiecemealInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[007:046) Module moduleWithExtsUsingPiecemealInheritance. Type: module. Declaration start char: 0, length: 229
  extensionConfigs: {
    k8s: {
      kubeConfig: k8s.config.kubeConfig
      namespace: k8s.config.namespace
    }
  }
}

module moduleWithExtsUsingPiecemealInheritanceLooped 'child/hasConfigurableExtensionsWithAlias.bicep' = [for i in range(0, 4): {
//@[109:110) Local i. Type: int. Declaration start char: 109, length: 1
//@[007:052) Module moduleWithExtsUsingPiecemealInheritanceLooped. Type: module[]. Declaration start char: 0, length: 315
  name: 'moduleWithExtsPiecemealInheritanceLooped${i}'
  extensionConfigs: {
    k8s: {
      kubeConfig: k8s.config.kubeConfig
      namespace: k8s.config.namespace
    }
  }
}]

module moduleExtConfigsConditionalMixed 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[007:039) Module moduleExtConfigsConditionalMixed. Type: module. Declaration start char: 0, length: 296
  extensionConfigs: {
    k8s: {
      kubeConfig: boolParam1 ? secureStrParam1 : k8s.config.kubeConfig
      namespace: boolParam1 ? az.resourceGroup().location : k8s.config.namespace
    }
  }
}

module moduleWithExtsEmpty 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[007:026) Module moduleWithExtsEmpty. Type: module. Declaration start char: 0, length: 162
  extensionConfigs: {
    k8s: k8s.config
    extWithOptionalConfig: {}
  }
}

// END: Extension configs for modules

