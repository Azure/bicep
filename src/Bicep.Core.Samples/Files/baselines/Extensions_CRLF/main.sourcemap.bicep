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

extension kubernetes with {
//@    "k8s": {
//@      "name": "Kubernetes",
//@      "version": "1.0.0",
//@      "config": {
//@        "kubeConfig": {
//@          "type": "string",
//@        },
//@        "namespace": {
//@          "type": "string",
//@        }
//@      }
//@    }
  kubeConfig: 'DELETE'
//@          "defaultValue": "DELETE"
  namespace: 'DELETE'
//@          "defaultValue": "DELETE"
} as k8s

//extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1:1.2.3' as graph

// END: Extension declarations

// BEGIN: Key vaults

resource kv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@    "kv1": {
//@      "existing": true,
//@      "type": "Microsoft.KeyVault/vaults",
//@      "apiVersion": "2019-09-01",
//@      "name": "kv1"
//@    },
  name: 'kv1'
}

resource scopedKv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@    "scopedKv1": {
//@      "existing": true,
//@      "type": "Microsoft.KeyVault/vaults",
//@      "apiVersion": "2019-09-01",
//@      "resourceGroup": "otherGroup",
//@      "name": "scopedKv1"
//@    },
  name: 'scopedKv1'
  scope: resourceGroup('otherGroup')
}

// END: Key vaults

// BEGIN: Extension configs for modules

module moduleWithExtsWithAliases 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@    "moduleWithExtsWithAliases": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2022-09-01",
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
//@              "Extensibility",
//@              "Enable defining extension configs for modules"
//@            ],
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "16109138385919213017"
//@            }
//@          },
//@          "extensions": {
//@            "k8s": {
//@              "name": "Kubernetes",
//@              "version": "1.0.0",
//@              "config": {
//@                "kubeConfig": {
//@                  "type": "string",
//@                  "defaultValue": "DELETE"
//@                },
//@                "namespace": {
//@                  "type": "string",
//@                  "defaultValue": "DELETE"
//@                }
//@              }
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
      kubeConfig: 'kubeConfig2FromModule'
//@            "kubeConfig": {
//@              "value": "kubeConfig2FromModule"
//@            },
      namespace: 'ns2FromModule'
//@            "namespace": {
//@              "value": "ns2FromModule"
//@            }
    }
  }
}

module moduleWithExtsWithoutAliases 'child/hasConfigurableExtensionsWithoutAlias.bicep' = {
//@    "moduleWithExtsWithoutAliases": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2022-09-01",
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
//@              "Extensibility",
//@              "Enable defining extension configs for modules"
//@            ],
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "15982584485385996989"
//@            }
//@          },
//@          "extensions": {
//@            "kubernetes": {
//@              "name": "Kubernetes",
//@              "version": "1.0.0",
//@              "config": {
//@                "kubeConfig": {
//@                  "type": "string",
//@                  "defaultValue": "DELETE"
//@                },
//@                "namespace": {
//@                  "type": "string",
//@                  "defaultValue": "DELETE"
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
      kubeConfig: 'kubeConfig2FromModule'
//@            "kubeConfig": {
//@              "value": "kubeConfig2FromModule"
//@            },
      namespace: 'ns2FromModule'
//@            "namespace": {
//@              "value": "ns2FromModule"
//@            }
    }
  }
}

module moduleExtConfigsFromParams 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@    "moduleExtConfigsFromParams": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2022-09-01",
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
//@              "Extensibility",
//@              "Enable defining extension configs for modules"
//@            ],
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "16109138385919213017"
//@            }
//@          },
//@          "extensions": {
//@            "k8s": {
//@              "name": "Kubernetes",
//@              "version": "1.0.0",
//@              "config": {
//@                "kubeConfig": {
//@                  "type": "string",
//@                  "defaultValue": "DELETE"
//@                },
//@                "namespace": {
//@                  "type": "string",
//@                  "defaultValue": "DELETE"
//@                }
//@              }
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

// TODO(kylealbert): Allow key vault references in extension configs
// module moduleExtConfigFromKeyVaultReference 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//   name: 'moduleExtConfigKeyVaultReference'
//   extensionConfigs: {
//     k8s: {
//       kubeConfig: kv1.getSecret('myKubeConfig'),
//       namespace: scopedKv1.getSecret('myNamespace')
//     }
//   }
// }

module moduleWithExtsUsingFullInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@    "moduleWithExtsUsingFullInheritance": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2022-09-01",
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
//@              "Extensibility",
//@              "Enable defining extension configs for modules"
//@            ],
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "16109138385919213017"
//@            }
//@          },
//@          "extensions": {
//@            "k8s": {
//@              "name": "Kubernetes",
//@              "version": "1.0.0",
//@              "config": {
//@                "kubeConfig": {
//@                  "type": "string",
//@                  "defaultValue": "DELETE"
//@                },
//@                "namespace": {
//@                  "type": "string",
//@                  "defaultValue": "DELETE"
//@                }
//@              }
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
//@          "k8s": "[extensionConfigs('k8s')]"
  }
}

module moduleWithExtsUsingPiecemealInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@    "moduleWithExtsUsingPiecemealInheritance": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2022-09-01",
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
//@              "Extensibility",
//@              "Enable defining extension configs for modules"
//@            ],
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "16109138385919213017"
//@            }
//@          },
//@          "extensions": {
//@            "k8s": {
//@              "name": "Kubernetes",
//@              "version": "1.0.0",
//@              "config": {
//@                "kubeConfig": {
//@                  "type": "string",
//@                  "defaultValue": "DELETE"
//@                },
//@                "namespace": {
//@                  "type": "string",
//@                  "defaultValue": "DELETE"
//@                }
//@              }
//@            }
//@          },
//@          "resources": {}
//@        }
//@      }
//@    }
  name: 'moduleWithExtsPiecemealInheritance'
//@      "name": "moduleWithExtsPiecemealInheritance",
  extensionConfigs: {
//@        "extensionConfigs": {
//@        },
    k8s: {
//@          "k8s": {
//@          }
      kubeConfig: k8s.config.kubeConfig
//@            "kubeConfig": "[extensionConfigs('k8s').kubeConfig]",
      namespace: k8s.config.namespace
//@            "namespace": "[extensionConfigs('k8s').namespace]"
    }
  }
}

// TODO(kylealbert): Figure out if this is allowable
// var k8sConfigDeployTime = {
//   kubeConfig: k8s.config.kubeConfig
//   namespace: strParam1
// }

// module moduleWithExtsUsingVar 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//   name: 'moduleWithExtsUsingVar'
//   extensionConfigs: {
//     k8s: k8sConfigDeployTime
//   }
// }

// END: Extension configs for modules

