// BEGIN: Parameters
//@[000:3483) ProgramExpression
//@[000:0000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:0000) | | └─ResourceReferenceExpression [UNPARENTED]
//@[000:0000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:0000) | | └─ResourceReferenceExpression [UNPARENTED]

param strParam1 string
//@[000:0022) ├─DeclaredParameterExpression { Name = strParam1 }
//@[016:0022) | └─AmbientTypeReferenceExpression { Name = string }

@secure()
//@[000:0039) ├─DeclaredParameterExpression { Name = secureStrParam1 }
//@[001:0009) | ├─FunctionCallExpression { Name = secure }
param secureStrParam1 string
//@[022:0028) | └─AmbientTypeReferenceExpression { Name = string }

param boolParam1 bool
//@[000:0021) ├─DeclaredParameterExpression { Name = boolParam1 }
//@[017:0021) | └─AmbientTypeReferenceExpression { Name = bool }

// END: Parameters

// BEGIN: Extension declarations

extension az
//@[000:0012) ├─ExtensionExpression { Name = az }
extension kubernetes as k8s
//@[000:0027) ├─ExtensionExpression { Name = k8s }

//extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1:1.2.3' as graph

// END: Extension declarations

// BEGIN: Key vaults

resource kv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[000:0082) ├─DeclaredResourceExpression
//@[063:0082) | └─ObjectExpression
  name: 'kv1'
}

resource scopedKv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[000:0135) ├─DeclaredResourceExpression
//@[069:0135) | └─ObjectExpression
  name: 'scopedKv1'
  scope: az.resourceGroup('otherGroup')
}

// END: Key vaults

// BEGIN: Test resources

resource testResource1 'az:My.Rp/TestType@2020-01-01' = {
//@[000:0103) ├─DeclaredResourceExpression
//@[056:0103) | └─ObjectExpression
  name: 'testResource1'
  properties: {}
//@[002:0016) |   └─ObjectPropertyExpression
//@[002:0012) |     ├─StringLiteralExpression { Value = properties }
//@[014:0016) |     └─ObjectExpression
}

resource aks 'Microsoft.ContainerService/managedClusters@2024-02-01' = {
//@[000:0138) ├─DeclaredResourceExpression
//@[071:0138) | └─ObjectExpression
  name: 'aksCluster'
  location: az.resourceGroup().location
//@[002:0039) |   └─ObjectPropertyExpression
//@[002:0010) |     ├─StringLiteralExpression { Value = location }
//@[012:0039) |     └─PropertyAccessExpression { PropertyName = location }
//@[012:0030) |       └─FunctionCallExpression { Name = resourceGroup }
}

// END: Test resources

// BEGIN: Extension configs for modules

module moduleWithExtsWithAliases 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:0229) ├─DeclaredModuleExpression
//@[084:0229) | ├─ObjectExpression
  name: 'moduleWithExtsWithAliases'
//@[002:0035) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0035) | |   └─StringLiteralExpression { Value = moduleWithExtsWithAliases }
  extensionConfigs: {
//@[020:0102) | └─ObjectExpression
    k8s: {
//@[004:0074) |   └─ObjectPropertyExpression
//@[004:0007) |     ├─StringLiteralExpression { Value = k8s }
//@[009:0074) |     └─ObjectExpression
      kubeConfig: 'kubeConfig2'
//@[006:0031) |       ├─ObjectPropertyExpression
//@[006:0016) |       | ├─StringLiteralExpression { Value = kubeConfig }
//@[018:0031) |       | └─StringLiteralExpression { Value = kubeConfig2 }
      namespace: 'ns2'
//@[006:0022) |       └─ObjectPropertyExpression
//@[006:0015) |         ├─StringLiteralExpression { Value = namespace }
//@[017:0022) |         └─StringLiteralExpression { Value = ns2 }
    }
  }
}

module moduleWithExtsWithoutAliases 'child/hasConfigurableExtensionsWithoutAlias.bicep' = {
//@[000:0221) ├─DeclaredModuleExpression
//@[090:0221) | ├─ObjectExpression
  name: 'moduleWithExtsWithoutAliases'
//@[002:0038) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0038) | |   └─StringLiteralExpression { Value = moduleWithExtsWithoutAliases }
  extensionConfigs: {
//@[020:0085) | └─ObjectExpression
    kubernetes: {
//@[004:0057) |   └─ObjectPropertyExpression
//@[004:0014) |     ├─StringLiteralExpression { Value = kubernetes }
//@[016:0057) |     └─ObjectExpression
      kubeConfig: 'kubeConfig2'
//@[006:0031) |       └─ObjectPropertyExpression
//@[006:0016) |         ├─StringLiteralExpression { Value = kubeConfig }
//@[018:0031) |         └─StringLiteralExpression { Value = kubeConfig2 }
    }
  }
}

module moduleExtConfigsFromParams 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:0289) ├─DeclaredModuleExpression
//@[085:0289) | ├─ObjectExpression
  name: 'moduleExtConfigsFromParams'
//@[002:0036) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0036) | |   └─StringLiteralExpression { Value = moduleExtConfigsFromParams }
  extensionConfigs: {
//@[020:0160) | └─ObjectExpression
    k8s: {
//@[004:0132) |   └─ObjectPropertyExpression
//@[004:0007) |     ├─StringLiteralExpression { Value = k8s }
//@[009:0132) |     └─ObjectExpression
      kubeConfig: boolParam1 ? secureStrParam1 : strParam1
//@[006:0058) |       ├─ObjectPropertyExpression
//@[006:0016) |       | ├─StringLiteralExpression { Value = kubeConfig }
//@[018:0058) |       | └─TernaryExpression
//@[018:0028) |       |   ├─ParametersReferenceExpression { Parameter = boolParam1 }
//@[031:0046) |       |   ├─ParametersReferenceExpression { Parameter = secureStrParam1 }
//@[049:0058) |       |   └─ParametersReferenceExpression { Parameter = strParam1 }
      namespace: boolParam1 ? strParam1 : 'falseCond'
//@[006:0053) |       └─ObjectPropertyExpression
//@[006:0015) |         ├─StringLiteralExpression { Value = namespace }
//@[017:0053) |         └─TernaryExpression
//@[017:0027) |           ├─ParametersReferenceExpression { Parameter = boolParam1 }
//@[030:0039) |           ├─ParametersReferenceExpression { Parameter = strParam1 }
//@[042:0053) |           └─StringLiteralExpression { Value = falseCond }
    }
  }
}

module moduleExtConfigFromKeyVaultReference 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:0267) ├─DeclaredModuleExpression
//@[095:0267) | ├─ObjectExpression
  name: 'moduleExtConfigKeyVaultReference'
//@[002:0042) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0042) | |   └─StringLiteralExpression { Value = moduleExtConfigKeyVaultReference }
  extensionConfigs: {
//@[020:0122) | └─ObjectExpression
    k8s: {
//@[004:0094) |   └─ObjectPropertyExpression
//@[004:0007) |     ├─StringLiteralExpression { Value = k8s }
//@[009:0094) |     └─ObjectExpression
      kubeConfig: kv1.getSecret('myKubeConfig')
//@[006:0047) |       ├─ObjectPropertyExpression
//@[006:0016) |       | ├─StringLiteralExpression { Value = kubeConfig }
//@[018:0047) |       | └─ResourceFunctionCallExpression { Name = getSecret }
//@[018:0021) |       |   ├─ResourceReferenceExpression
//@[032:0046) |       |   └─StringLiteralExpression { Value = myKubeConfig }
      namespace: 'default'
//@[006:0026) |       └─ObjectPropertyExpression
//@[006:0015) |         ├─StringLiteralExpression { Value = namespace }
//@[017:0026) |         └─StringLiteralExpression { Value = default }
    }
  }
}

module moduleExtConfigFromReferences 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:0306) ├─DeclaredModuleExpression
//@[088:0306) | ├─ObjectExpression
  name: 'moduleExtConfigFromReferences'
//@[002:0039) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0039) | |   └─StringLiteralExpression { Value = moduleExtConfigFromReferences }
  extensionConfigs: {
//@[020:0171) | └─ObjectExpression
    k8s: {
//@[004:0143) |   └─ObjectPropertyExpression
//@[004:0007) |     ├─StringLiteralExpression { Value = k8s }
//@[009:0143) |     └─ObjectExpression
      kubeConfig: aks.listClusterAdminCredential().kubeconfigs[0].value
//@[006:0071) |       ├─ObjectPropertyExpression
//@[006:0016) |       | ├─StringLiteralExpression { Value = kubeConfig }
//@[018:0071) |       | └─PropertyAccessExpression { PropertyName = value }
//@[018:0065) |       |   └─ArrayAccessExpression
//@[063:0064) |       |     ├─IntegerLiteralExpression { Value = 0 }
//@[018:0062) |       |     └─PropertyAccessExpression { PropertyName = kubeconfigs }
//@[018:0050) |       |       └─ResourceFunctionCallExpression { Name = listClusterAdminCredential }
//@[018:0021) |       |         └─ResourceReferenceExpression
      namespace: testResource1.properties.namespace
//@[006:0051) |       └─ObjectPropertyExpression
//@[006:0015) |         ├─StringLiteralExpression { Value = namespace }
//@[017:0051) |         └─PropertyAccessExpression { PropertyName = namespace }
//@[017:0041) |           └─PropertyAccessExpression { PropertyName = properties }
//@[017:0030) |             └─ResourceReferenceExpression
    }
  }
}

module moduleWithExtsUsingFullInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:0187) ├─DeclaredModuleExpression
//@[093:0187) | ├─ObjectExpression
  name: 'moduleWithExtsFullInheritance'
//@[002:0039) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0039) | |   └─StringLiteralExpression { Value = moduleWithExtsFullInheritance }
  extensionConfigs: {
//@[020:0047) | └─ObjectExpression
    k8s: k8s.config
//@[004:0019) |   └─ObjectPropertyExpression
//@[004:0007) |     ├─StringLiteralExpression { Value = k8s }
//@[009:0019) |     └─PropertyAccessExpression { PropertyName = config }
//@[009:0012) |       └─ExtensionReferenceExpression { ExtensionAlias = k8s }
  }
}

module moduleWithExtsUsingPiecemealInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:0275) ├─DeclaredModuleExpression
//@[098:0275) | ├─ObjectExpression
  name: 'moduleWithExtsPiecemealInheritance'
//@[002:0044) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0044) | |   └─StringLiteralExpression { Value = moduleWithExtsPiecemealInheritance }
  extensionConfigs: {
//@[020:0125) | └─ObjectExpression
    k8s: {
//@[004:0097) |   └─ObjectPropertyExpression
//@[004:0007) |     ├─StringLiteralExpression { Value = k8s }
//@[009:0097) |     └─ObjectExpression
      kubeConfig: k8s.config.kubeConfig
//@[006:0039) |       ├─ObjectPropertyExpression
//@[006:0016) |       | ├─StringLiteralExpression { Value = kubeConfig }
//@[018:0039) |       | └─PropertyAccessExpression { PropertyName = kubeConfig }
//@[018:0028) |       |   └─PropertyAccessExpression { PropertyName = config }
//@[018:0021) |       |     └─ExtensionReferenceExpression { ExtensionAlias = k8s }
      namespace: k8s.config.namespace
//@[006:0037) |       └─ObjectPropertyExpression
//@[006:0015) |         ├─StringLiteralExpression { Value = namespace }
//@[017:0037) |         └─PropertyAccessExpression { PropertyName = namespace }
//@[017:0027) |           └─PropertyAccessExpression { PropertyName = config }
//@[017:0020) |             └─ExtensionReferenceExpression { ExtensionAlias = k8s }
    }
  }
}

module moduleWithExtsUsingPiecemealInheritanceLooped 'child/hasConfigurableExtensionsWithAlias.bicep' = [for i in range(0, 4): {
//@[000:0315) ├─DeclaredModuleExpression
//@[104:0315) | ├─ForLoopExpression
//@[114:0125) | | ├─FunctionCallExpression { Name = range }
//@[120:0121) | | | ├─IntegerLiteralExpression { Value = 0 }
//@[123:0124) | | | └─IntegerLiteralExpression { Value = 4 }
//@[127:0314) | | └─ObjectExpression
//@[114:0125) | |         └─FunctionCallExpression { Name = range }
//@[120:0121) | |           ├─IntegerLiteralExpression { Value = 0 }
//@[123:0124) | |           └─IntegerLiteralExpression { Value = 4 }
  name: 'moduleWithExtsPiecemealInheritanceLooped${i}'
//@[002:0054) | |   └─ObjectPropertyExpression
//@[002:0006) | |     ├─StringLiteralExpression { Value = name }
//@[008:0054) | |     └─InterpolatedStringExpression
//@[051:0052) | |       └─ArrayAccessExpression
//@[051:0052) | |         ├─CopyIndexExpression
  extensionConfigs: {
//@[020:0125) | └─ObjectExpression
    k8s: {
//@[004:0097) |   └─ObjectPropertyExpression
//@[004:0007) |     ├─StringLiteralExpression { Value = k8s }
//@[009:0097) |     └─ObjectExpression
      kubeConfig: k8s.config.kubeConfig
//@[006:0039) |       ├─ObjectPropertyExpression
//@[006:0016) |       | ├─StringLiteralExpression { Value = kubeConfig }
//@[018:0039) |       | └─PropertyAccessExpression { PropertyName = kubeConfig }
//@[018:0028) |       |   └─PropertyAccessExpression { PropertyName = config }
//@[018:0021) |       |     └─ExtensionReferenceExpression { ExtensionAlias = k8s }
      namespace: k8s.config.namespace
//@[006:0037) |       └─ObjectPropertyExpression
//@[006:0015) |         ├─StringLiteralExpression { Value = namespace }
//@[017:0037) |         └─PropertyAccessExpression { PropertyName = namespace }
//@[017:0027) |           └─PropertyAccessExpression { PropertyName = config }
//@[017:0020) |             └─ExtensionReferenceExpression { ExtensionAlias = k8s }
    }
  }
}]

module moduleExtConfigsConditionalMixed 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:0359) └─DeclaredModuleExpression
//@[091:0359)   ├─ObjectExpression
  name: 'moduleExtConfigsConditionalMixedValueAndInheritance'
//@[002:0061)   | └─ObjectPropertyExpression
//@[002:0006)   |   ├─StringLiteralExpression { Value = name }
//@[008:0061)   |   └─StringLiteralExpression { Value = moduleExtConfigsConditionalMixedValueAndInheritance }
  extensionConfigs: {
//@[020:0199)   └─ObjectExpression
    k8s: {
//@[004:0171)     └─ObjectPropertyExpression
//@[004:0007)       ├─StringLiteralExpression { Value = k8s }
//@[009:0171)       └─ObjectExpression
      kubeConfig: boolParam1 ? secureStrParam1 : k8s.config.kubeConfig
//@[006:0070)         ├─ObjectPropertyExpression
//@[006:0016)         | ├─StringLiteralExpression { Value = kubeConfig }
//@[018:0070)         | └─TernaryExpression
//@[018:0028)         |   ├─ParametersReferenceExpression { Parameter = boolParam1 }
//@[031:0046)         |   ├─ParametersReferenceExpression { Parameter = secureStrParam1 }
//@[049:0070)         |   └─PropertyAccessExpression { PropertyName = kubeConfig }
//@[049:0059)         |     └─PropertyAccessExpression { PropertyName = config }
//@[049:0052)         |       └─ExtensionReferenceExpression { ExtensionAlias = k8s }
      namespace: boolParam1 ? az.resourceGroup().location : k8s.config.namespace
//@[006:0080)         └─ObjectPropertyExpression
//@[006:0015)           ├─StringLiteralExpression { Value = namespace }
//@[017:0080)           └─TernaryExpression
//@[017:0027)             ├─ParametersReferenceExpression { Parameter = boolParam1 }
//@[030:0057)             ├─PropertyAccessExpression { PropertyName = location }
//@[030:0048)             | └─FunctionCallExpression { Name = resourceGroup }
//@[060:0080)             └─PropertyAccessExpression { PropertyName = namespace }
//@[060:0070)               └─PropertyAccessExpression { PropertyName = config }
//@[060:0063)                 └─ExtensionReferenceExpression { ExtensionAlias = k8s }
    }
  }
}

// END: Extension configs for modules

