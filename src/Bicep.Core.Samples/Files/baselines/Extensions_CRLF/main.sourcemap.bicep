// BEGIN: Parameters

param strParam1 string
//@    "strParam1": {
//@      "type": "string"
//@    },

@secure()
//@      "type": "securestring"
param secureStrParam1 string
//@    "secureStrParam1": {
//@    },

param boolParam1 bool
//@    "boolParam1": {
//@      "type": "bool"
//@    }

// END: Parameters

// BEGIN: Extension declarations

extension az
//@    "az": {
//@      "name": "AzureResourceManager",
//@      "version": "0.2.784"
//@    },
extension kubernetes as k8s
//@    "k8s": {
//@      "name": "Kubernetes",
//@      "version": "1.0.0"
//@    }

//extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1:1.2.3' as graph

// END: Extension declarations

// BEGIN: Key vaults

resource kv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@    "kv1": {
//@      "existing": true,
//@      "extension": "az",
//@      "type": "Microsoft.KeyVault/vaults",
//@      "apiVersion": "2019-09-01",
//@      "name": "kv1"
//@    },
  name: 'kv1'
}

resource scopedKv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@    "scopedKv1": {
//@      "existing": true,
//@      "extension": "az",
//@      "type": "Microsoft.KeyVault/vaults",
//@      "apiVersion": "2019-09-01",
//@      "resourceGroup": "otherGroup",
//@      "name": "scopedKv1"
//@    },
  name: 'scopedKv1'
  scope: az.resourceGroup('otherGroup')
}

// END: Key vaults

// BEGIN: Test resources

resource testResource1 'az:My.Rp/TestType@2020-01-01' = {
//@    "testResource1": {
//@      "extension": "az",
//@      "type": "My.Rp/TestType",
//@      "apiVersion": "2020-01-01",
//@      "name": "testResource1",
//@    },
  name: 'testResource1'
  properties: {}
//@      "properties": {}
}

resource aks 'Microsoft.ContainerService/managedClusters@2024-02-01' = {
//@    "aks": {
//@      "extension": "az",
//@      "type": "Microsoft.ContainerService/managedClusters",
//@      "apiVersion": "2024-02-01",
//@      "name": "aksCluster",
//@    },
  name: 'aksCluster'
  location: az.resourceGroup().location
//@      "location": "[resourceGroup().location]"
}

// END: Test resources

// BEGIN: Extension configs for modules

module moduleWithExtsWithAliases 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@    "moduleWithExtsWithAliases": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-03-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "languageVersion": "2.2-experimental",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_EXPERIMENTAL_WARNING": "This template uses ARM features that are experimental. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.",
//@            "_EXPERIMENTAL_FEATURES_ENABLED": [
//@              "Enable defining extension configs for modules"
//@            ],
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "15850125793643788204"
//@            }
//@          },
//@          "extensions": {
//@            "k8s": {
//@              "name": "Kubernetes",
//@              "version": "1.0.0"
//@            }
//@          },
//@          "resources": {}
//@        }
//@      }
//@    },
  name: 'moduleWithExtsWithAliases'
//@      "name": "moduleWithExtsWithAliases",
  extensionConfigs: {
//@        "extensionConfigs": {
//@        },
    k8s: {
//@          "k8s": {
//@          }
      kubeConfig: 'kubeConfig2'
//@            "kubeConfig": {
//@              "value": "kubeConfig2"
//@            },
      namespace: 'ns2'
//@            "namespace": {
//@              "value": "ns2"
//@            }
    }
  }
}

module moduleWithExtsWithoutAliases 'child/hasConfigurableExtensionsWithoutAlias.bicep' = {
//@    "moduleWithExtsWithoutAliases": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-03-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "languageVersion": "2.2-experimental",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_EXPERIMENTAL_WARNING": "This template uses ARM features that are experimental. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.",
//@            "_EXPERIMENTAL_FEATURES_ENABLED": [
//@              "Enable defining extension configs for modules"
//@            ],
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "12485839995628084055"
//@            }
//@          },
//@          "extensions": {
//@            "kubernetes": {
//@              "name": "Kubernetes",
//@              "version": "1.0.0",
//@              "config": {
//@                "namespace": {
//@                  "defaultValue": "nsInsideModule"
//@                }
//@              }
//@            }
//@          },
//@          "resources": {}
//@        }
//@      }
//@    },
  name: 'moduleWithExtsWithoutAliases'
//@      "name": "moduleWithExtsWithoutAliases",
  extensionConfigs: {
//@        "extensionConfigs": {
//@        },
    kubernetes: {
//@          "kubernetes": {
//@          }
      kubeConfig: 'kubeConfig2'
//@            "kubeConfig": {
//@              "value": "kubeConfig2"
//@            }
    }
  }
}

module moduleExtConfigsFromParams 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@    "moduleExtConfigsFromParams": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-03-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "languageVersion": "2.2-experimental",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_EXPERIMENTAL_WARNING": "This template uses ARM features that are experimental. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.",
//@            "_EXPERIMENTAL_FEATURES_ENABLED": [
//@              "Enable defining extension configs for modules"
//@            ],
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "15850125793643788204"
//@            }
//@          },
//@          "extensions": {
//@            "k8s": {
//@              "name": "Kubernetes",
//@              "version": "1.0.0"
//@            }
//@          },
//@          "resources": {}
//@        }
//@      }
//@    },
  name: 'moduleExtConfigsFromParams'
//@      "name": "moduleExtConfigsFromParams",
  extensionConfigs: {
//@        "extensionConfigs": {
//@        },
    k8s: {
//@          "k8s": {
//@          }
      kubeConfig: boolParam1 ? secureStrParam1 : strParam1
//@            "kubeConfig": "[if(parameters('boolParam1'), createObject('value', parameters('secureStrParam1')), createObject('value', parameters('strParam1')))]",
      namespace: boolParam1 ? strParam1 : 'falseCond'
//@            "namespace": "[if(parameters('boolParam1'), createObject('value', parameters('strParam1')), createObject('value', 'falseCond'))]"
    }
  }
}

module moduleExtConfigFromKeyVaultReference 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@    "moduleExtConfigFromKeyVaultReference": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-03-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "languageVersion": "2.2-experimental",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_EXPERIMENTAL_WARNING": "This template uses ARM features that are experimental. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.",
//@            "_EXPERIMENTAL_FEATURES_ENABLED": [
//@              "Enable defining extension configs for modules"
//@            ],
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "15850125793643788204"
//@            }
//@          },
//@          "extensions": {
//@            "k8s": {
//@              "name": "Kubernetes",
//@              "version": "1.0.0"
//@            }
//@          },
//@          "resources": {}
//@        }
//@      }
//@    },
  name: 'moduleExtConfigKeyVaultReference'
//@      "name": "moduleExtConfigKeyVaultReference",
  extensionConfigs: {
//@        "extensionConfigs": {
//@        },
    k8s: {
//@          "k8s": {
//@          }
      kubeConfig: kv1.getSecret('myKubeConfig')
//@            "kubeConfig": {
//@              "keyVaultReference": {
//@                "keyVault": {
//@                  "id": "[resourceId('Microsoft.KeyVault/vaults', 'kv1')]"
//@                },
//@                "secretName": "myKubeConfig"
//@              }
//@            },
      namespace: 'default'
//@            "namespace": {
//@              "value": "default"
//@            }
    }
  }
}

module moduleExtConfigFromReferences 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@    "moduleExtConfigFromReferences": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-03-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "languageVersion": "2.2-experimental",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_EXPERIMENTAL_WARNING": "This template uses ARM features that are experimental. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.",
//@            "_EXPERIMENTAL_FEATURES_ENABLED": [
//@              "Enable defining extension configs for modules"
//@            ],
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "15850125793643788204"
//@            }
//@          },
//@          "extensions": {
//@            "k8s": {
//@              "name": "Kubernetes",
//@              "version": "1.0.0"
//@            }
//@          },
//@          "resources": {}
//@        }
//@      },
//@      "dependsOn": [
//@        "aks",
//@        "testResource1"
//@      ]
//@    },
  name: 'moduleExtConfigFromReferences'
//@      "name": "moduleExtConfigFromReferences",
  extensionConfigs: {
//@        "extensionConfigs": {
//@        },
    k8s: {
//@          "k8s": {
//@          }
      kubeConfig: aks.listClusterAdminCredential().kubeconfigs[0].value
//@            "kubeConfig": {
//@              "value": "[listClusterAdminCredential('aks', '2024-02-01').kubeconfigs[0].value]"
//@            },
      namespace: testResource1.properties.namespace
//@            "namespace": {
//@              "value": "[reference('testResource1').namespace]"
//@            }
    }
  }
}

module moduleWithExtsUsingFullInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@    "moduleWithExtsUsingFullInheritance": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-03-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "languageVersion": "2.2-experimental",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_EXPERIMENTAL_WARNING": "This template uses ARM features that are experimental. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.",
//@            "_EXPERIMENTAL_FEATURES_ENABLED": [
//@              "Enable defining extension configs for modules"
//@            ],
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "15850125793643788204"
//@            }
//@          },
//@          "extensions": {
//@            "k8s": {
//@              "name": "Kubernetes",
//@              "version": "1.0.0"
//@            }
//@          },
//@          "resources": {}
//@        }
//@      }
//@    },
  name: 'moduleWithExtsFullInheritance'
//@      "name": "moduleWithExtsFullInheritance",
  extensionConfigs: {
//@        "extensionConfigs": {
//@        },
    k8s: k8s.config
//@          "k8s": "[extensions('k8s').config]"
  }
}

module moduleWithExtsUsingPiecemealInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@    "moduleWithExtsUsingPiecemealInheritance": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-03-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "languageVersion": "2.2-experimental",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_EXPERIMENTAL_WARNING": "This template uses ARM features that are experimental. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.",
//@            "_EXPERIMENTAL_FEATURES_ENABLED": [
//@              "Enable defining extension configs for modules"
//@            ],
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "15850125793643788204"
//@            }
//@          },
//@          "extensions": {
//@            "k8s": {
//@              "name": "Kubernetes",
//@              "version": "1.0.0"
//@            }
//@          },
//@          "resources": {}
//@        }
//@      }
//@    },
  name: 'moduleWithExtsPiecemealInheritance'
//@      "name": "moduleWithExtsPiecemealInheritance",
  extensionConfigs: {
//@        "extensionConfigs": {
//@        },
    k8s: {
//@          "k8s": {
//@          }
      kubeConfig: k8s.config.kubeConfig
//@            "kubeConfig": "[extensions('k8s').config.kubeConfig]",
      namespace: k8s.config.namespace
//@            "namespace": "[extensions('k8s').config.namespace]"
    }
  }
}

module moduleWithExtsUsingPiecemealInheritanceLooped 'child/hasConfigurableExtensionsWithAlias.bicep' = [for i in range(0, 4): {
//@    "moduleWithExtsUsingPiecemealInheritanceLooped": {
//@      "copy": {
//@        "name": "moduleWithExtsUsingPiecemealInheritanceLooped",
//@        "count": "[length(range(0, 4))]"
//@      },
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-03-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "languageVersion": "2.2-experimental",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_EXPERIMENTAL_WARNING": "This template uses ARM features that are experimental. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.",
//@            "_EXPERIMENTAL_FEATURES_ENABLED": [
//@              "Enable defining extension configs for modules"
//@            ],
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "15850125793643788204"
//@            }
//@          },
//@          "extensions": {
//@            "k8s": {
//@              "name": "Kubernetes",
//@              "version": "1.0.0"
//@            }
//@          },
//@          "resources": {}
//@        }
//@      }
//@    },
  name: 'moduleWithExtsPiecemealInheritanceLooped${i}'
//@      "name": "[format('moduleWithExtsPiecemealInheritanceLooped{0}', range(0, 4)[copyIndex()])]",
  extensionConfigs: {
//@        "extensionConfigs": {
//@        },
    k8s: {
//@          "k8s": {
//@          }
      kubeConfig: k8s.config.kubeConfig
//@            "kubeConfig": "[extensions('k8s').config.kubeConfig]",
      namespace: k8s.config.namespace
//@            "namespace": "[extensions('k8s').config.namespace]"
    }
  }
}]

module moduleExtConfigsConditionalMixed 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@    "moduleExtConfigsConditionalMixed": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-03-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "languageVersion": "2.2-experimental",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_EXPERIMENTAL_WARNING": "This template uses ARM features that are experimental. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.",
//@            "_EXPERIMENTAL_FEATURES_ENABLED": [
//@              "Enable defining extension configs for modules"
//@            ],
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "15850125793643788204"
//@            }
//@          },
//@          "extensions": {
//@            "k8s": {
//@              "name": "Kubernetes",
//@              "version": "1.0.0"
//@            }
//@          },
//@          "resources": {}
//@        }
//@      }
//@    }
  name: 'moduleExtConfigsConditionalMixedValueAndInheritance'
//@      "name": "moduleExtConfigsConditionalMixedValueAndInheritance",
  extensionConfigs: {
//@        "extensionConfigs": {
//@        },
    k8s: {
//@          "k8s": {
//@          }
      kubeConfig: boolParam1 ? secureStrParam1 : k8s.config.kubeConfig
//@            "kubeConfig": "[if(parameters('boolParam1'), createObject('value', parameters('secureStrParam1')), extensions('k8s').config.kubeConfig)]",
      namespace: boolParam1 ? az.resourceGroup().location : k8s.config.namespace
//@            "namespace": "[if(parameters('boolParam1'), createObject('value', resourceGroup().location), extensions('k8s').config.namespace)]"
    }
  }
}

// END: Extension configs for modules

