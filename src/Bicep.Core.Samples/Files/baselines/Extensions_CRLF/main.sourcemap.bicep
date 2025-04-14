// BEGIN: Extension declarations

// extension kubernetes with {
//   kubeConfig: 'DELETE'
//   namespace: 'DELETE'
// } as k8s

//extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0:0.1.8-preview' as graph

// END: Extension declarations

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
//@    }
  name: 'moduleWithExtsWithoutAlaises'
//@      "name": "moduleWithExtsWithoutAlaises",
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

// END: Extension configs for modules

