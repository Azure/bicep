// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net.Http;
using System.Text;
using System.Text.Json;
using Bicep.Core.Registry.Catalog.Implementation;
using Bicep.Core.Registry.Catalog.Implementation.PublicRegistries;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;

namespace Bicep.Core.UnitTests.Registry.Catalog
{
    [TestClass]
    public class PublicModuleMetadataProviderTests
    {
        private PublicModuleMetadataHttpClient CreateTypedClient()
        {
            var httpClient = MockHttpMessageHandler.ToHttpClient();
            return new PublicModuleMetadataHttpClient(httpClient);
        }

        private const string ModuleIndexJson = """
            [
              {
                "moduleName": "ai/bing-resource",
                "tags": [
                  "1.0.1",
                  "1.0.2"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/ai/bing-resource/1.0.1/modules/ai/bing-resource/README.md"
                  },
                  "1.0.2": {
                    "description": "This module deploys Azure Bing Resource",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/ai/bing-resource/1.0.2/modules/ai/bing-resource/README.md"
                  }
                }
              },
              {
                "moduleName": "ai/cognitiveservices",
                "tags": [
                  "1.0.1",
                  "1.0.2",
                  "1.0.3",
                  "1.0.4",
                  "1.0.5",
                  "1.1.1"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/ai/cognitiveservices/1.0.1/modules/ai/cognitiveservices/README.md"
                  },
                  "1.0.2": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/ai/cognitiveservices/1.0.2/modules/ai/cognitiveservices/README.md"
                  },
                  "1.0.3": {
                    "description": "This module deploys CognitiveServices (Microsoft.CognitiveServices/accounts) and optionally available integrations.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/ai/cognitiveservices/1.0.3/modules/ai/cognitiveservices/README.md"
                  },
                  "1.0.4": {
                    "description": "This module deploys CognitiveServices (Microsoft.CognitiveServices/accounts) and optionally available integrations.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/ai/cognitiveservices/1.0.4/modules/ai/cognitiveservices/README.md"
                  },
                  "1.0.5": {
                    "description": "This module deploys CognitiveServices (Microsoft.CognitiveServices/accounts) and optionally available integrations.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/ai/cognitiveservices/1.0.5/modules/ai/cognitiveservices/README.md"
                  },
                  "1.1.1": {
                    "description": "This module deploys CognitiveServices (Microsoft.CognitiveServices/accounts) and optionally available integrations.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/ai/cognitiveservices/1.1.1/modules/ai/cognitiveservices/README.md"
                  }
                }
              },
              {
                "moduleName": "app/app-configuration",
                "tags": [
                  "1.0.1",
                  "1.1.1",
                  "1.1.2"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/app/app-configuration/1.0.1/modules/app/app-configuration/README.md"
                  },
                  "1.1.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/app/app-configuration/1.1.1/modules/app/app-configuration/README.md"
                  },
                  "1.1.2": {
                    "description": "Bicep module for simplified deployment Azure App Configuration resources and optionally available integrations.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/app/app-configuration/1.1.2/modules/app/app-configuration/README.md"
                  }
                }
              },
              {
                "moduleName": "app/dapr-containerapp",
                "tags": [
                  "1.0.1",
                  "1.0.2",
                  "1.0.3"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/app/dapr-containerapp/1.0.1/modules/app/dapr-containerapp/README.md"
                  },
                  "1.0.2": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/app/dapr-containerapp/1.0.2/modules/app/dapr-containerapp/README.md"
                  },
                  "1.0.3": {
                    "description": "A dapr optimised container app",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/app/dapr-containerapp/1.0.3/modules/app/dapr-containerapp/README.md"
                  }
                }
              },
              {
                "moduleName": "app/dapr-containerapps-environment",
                "tags": [
                  "1.0.1",
                  "1.1.1",
                  "1.2.1",
                  "1.2.2",
                  "1.2.3"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/app/dapr-containerapps-environment/1.0.1/modules/app/dapr-containerapps-environment/README.md"
                  },
                  "1.1.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/app/dapr-containerapps-environment/1.1.1/modules/app/dapr-containerapps-environment/README.md"
                  },
                  "1.2.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/app/dapr-containerapps-environment/1.2.1/modules/app/dapr-containerapps-environment/README.md"
                  },
                  "1.2.2": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/app/dapr-containerapps-environment/1.2.2/modules/app/dapr-containerapps-environment/README.md"
                  },
                  "1.2.3": {
                    "description": "Container Apps Environment for Dapr",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/app/dapr-containerapps-environment/1.2.3/modules/app/dapr-containerapps-environment/README.md"
                  }
                }
              },
              {
                "moduleName": "authorization/resource-scope-role-assignment",
                "tags": [
                  "1.0.1",
                  "1.0.2"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/authorization/resource-scope-role-assignment/1.0.1/modules/authorization/resource-scope-role-assignment/README.md"
                  },
                  "1.0.2": {
                    "description": "Create an Azure Authorization Role Assignment at the scope of a Resource E.g. on a Storage Container",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/authorization/resource-scope-role-assignment/1.0.2/modules/authorization/resource-scope-role-assignment/README.md"
                  }
                }
              },
              {
                "moduleName": "azure-gaming/game-dev-vm",
                "tags": [
                  "1.0.1",
                  "1.0.2",
                  "2.0.1",
                  "2.0.2",
                  "2.0.3"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/azure-gaming/game-dev-vm/1.0.1/modules/azure-gaming/game-dev-vm/README.md"
                  },
                  "1.0.2": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/azure-gaming/game-dev-vm/1.0.2/modules/azure-gaming/game-dev-vm/README.md"
                  },
                  "2.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/azure-gaming/game-dev-vm/2.0.1/modules/azure-gaming/game-dev-vm/README.md"
                  },
                  "2.0.2": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/azure-gaming/game-dev-vm/2.0.2/modules/azure-gaming/game-dev-vm/README.md"
                  },
                  "2.0.3": {
                    "description": "Bicep Module to simplify deployment of the Azure Game Developer VM",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/azure-gaming/game-dev-vm/2.0.3/modules/azure-gaming/game-dev-vm/README.md"
                  }
                }
              },
              {
                "moduleName": "azure-gaming/game-dev-vmss",
                "tags": [
                  "1.0.1",
                  "1.1.1",
                  "1.1.2"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/azure-gaming/game-dev-vmss/1.0.1/modules/azure-gaming/game-dev-vmss/README.md"
                  },
                  "1.1.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/azure-gaming/game-dev-vmss/1.1.1/modules/azure-gaming/game-dev-vmss/README.md"
                  },
                  "1.1.2": {
                    "description": "Bicep Module to simplify deployment of the Azure Game Developer VMSS",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/azure-gaming/game-dev-vmss/1.1.2/modules/azure-gaming/game-dev-vmss/README.md"
                  }
                }
              },
              {
                "moduleName": "compute/availability-set",
                "tags": [
                  "1.0.1",
                  "1.0.2"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/compute/availability-set/1.0.1/modules/compute/availability-set/README.md"
                  },
                  "1.0.2": {
                    "description": "This module deploys Microsoft.Compute Availability Sets and optionally available children or extensions",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/compute/availability-set/1.0.2/modules/compute/availability-set/README.md"
                  }
                }
              },
              {
                "moduleName": "compute/container-registry",
                "tags": [
                  "1.0.1",
                  "1.0.2",
                  "1.0.3",
                  "1.1.1"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/compute/container-registry/1.0.1/modules/compute/container-registry/README.md"
                  },
                  "1.0.2": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/compute/container-registry/1.0.2/modules/compute/container-registry/README.md"
                  },
                  "1.0.3": {
                    "description": "This module deploys Container Registry (Microsoft.ContainerRegistry/registries) and optionally available integrations.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/compute/container-registry/1.0.3/modules/compute/container-registry/README.md"
                  },
                  "1.1.1": {
                    "description": "This module deploys Container Registry (Microsoft.ContainerRegistry/registries) and optionally available integrations.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/compute/container-registry/1.1.1/modules/compute/container-registry/README.md"
                  }
                }
              },
              {
                "moduleName": "compute/custom-image-vmss",
                "tags": [
                  "1.0.1",
                  "1.0.2"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/compute/custom-image-vmss/1.0.1/modules/compute/custom-image-vmss/README.md"
                  },
                  "1.0.2": {
                    "description": "Create an Azure VMSS Cluster with a Custom Image to simplify creation of Marketplace Applications",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/compute/custom-image-vmss/1.0.2/modules/compute/custom-image-vmss/README.md"
                  }
                }
              },
              {
                "moduleName": "compute/event-hub",
                "tags": [
                  "0.0.1",
                  "1.0.1",
                  "2.0.1",
                  "2.0.2"
                ],
                "properties": {
                  "0.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/compute/event-hub/0.0.1/modules/compute/event-hub/README.md"
                  },
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/compute/event-hub/1.0.1/modules/compute/event-hub/README.md"
                  },
                  "2.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/compute/event-hub/2.0.1/modules/compute/event-hub/README.md"
                  },
                  "2.0.2": {
                    "description": "This module deploys Microsoft.data event clusters, event namespaces, event hubs and associated configurations.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/compute/event-hub/2.0.2/modules/compute/event-hub/README.md"
                  }
                }
              },
              {
                "moduleName": "compute/function-app",
                "tags": [
                  "1.0.1",
                  "1.1.1",
                  "1.1.2",
                  "2.0.1"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/compute/function-app/1.0.1/modules/compute/function-app/README.md"
                  },
                  "1.1.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/compute/function-app/1.1.1/modules/compute/function-app/README.md"
                  },
                  "1.1.2": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/compute/function-app/1.1.2/modules/compute/function-app/README.md"
                  },
                  "2.0.1": {
                    "description": "Module to create function app for your application",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/compute/function-app/2.0.1/modules/compute/function-app/README.md"
                  }
                }
              },
              {
                "moduleName": "cost/resourcegroup-scheduled-action",
                "tags": [
                  "1.0.1",
                  "1.0.2"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/cost/resourcegroup-scheduled-action/1.0.1/modules/cost/resourcegroup-scheduled-action/README.md"
                  },
                  "1.0.2": {
                    "description": "Creates a scheduled action to notify recipients about the latest costs on a recurring schedule.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/cost/resourcegroup-scheduled-action/1.0.2/modules/cost/resourcegroup-scheduled-action/README.md"
                  }
                }
              },
              {
                "moduleName": "cost/subscription-scheduled-action",
                "tags": [
                  "1.0.1",
                  "1.0.2"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/cost/subscription-scheduled-action/1.0.1/modules/cost/subscription-scheduled-action/README.md"
                  },
                  "1.0.2": {
                    "description": "Creates a scheduled action to notify recipients about the latest costs or when an anomaly is detected.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/cost/subscription-scheduled-action/1.0.2/modules/cost/subscription-scheduled-action/README.md"
                  }
                }
              },
              {
                "moduleName": "deployment-scripts/aks-run-command",
                "tags": [
                  "1.0.1",
                  "1.0.2",
                  "1.0.3",
                  "2.0.1",
                  "2.0.2"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/aks-run-command/1.0.1/modules/deployment-scripts/aks-run-command/README.md"
                  },
                  "1.0.2": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/aks-run-command/1.0.2/modules/deployment-scripts/aks-run-command/README.md"
                  },
                  "1.0.3": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/aks-run-command/1.0.3/modules/deployment-scripts/aks-run-command/README.md"
                  },
                  "2.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/aks-run-command/2.0.1/modules/deployment-scripts/aks-run-command/README.md"
                  },
                  "2.0.2": {
                    "description": "An Azure CLI Deployment Script that allows you to run a command on a Kubernetes cluster.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/aks-run-command/2.0.2/modules/deployment-scripts/aks-run-command/README.md"
                  }
                }
              },
              {
                "moduleName": "deployment-scripts/aks-run-helm",
                "tags": [
                  "1.0.1",
                  "2.0.1",
                  "2.0.2",
                  "2.0.3"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/aks-run-helm/1.0.1/modules/deployment-scripts/aks-run-helm/README.md"
                  },
                  "2.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/aks-run-helm/2.0.1/modules/deployment-scripts/aks-run-helm/README.md"
                  },
                  "2.0.2": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/aks-run-helm/2.0.2/modules/deployment-scripts/aks-run-helm/README.md"
                  },
                  "2.0.3": {
                    "description": "An Azure CLI Deployment Script that allows you to run a Helm command at a Kubernetes cluster.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/aks-run-helm/2.0.3/modules/deployment-scripts/aks-run-helm/README.md"
                  }
                }
              },
              {
                "moduleName": "deployment-scripts/build-acr",
                "tags": [
                  "1.0.1",
                  "1.0.2",
                  "2.0.1",
                  "2.0.2"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/build-acr/1.0.1/modules/deployment-scripts/build-acr/README.md"
                  },
                  "1.0.2": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/build-acr/1.0.2/modules/deployment-scripts/build-acr/README.md"
                  },
                  "2.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/build-acr/2.0.1/modules/deployment-scripts/build-acr/README.md"
                  },
                  "2.0.2": {
                    "description": "Builds a container image inside ACR from source code",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/build-acr/2.0.2/modules/deployment-scripts/build-acr/README.md"
                  }
                }
              },
              {
                "moduleName": "deployment-scripts/create-kv-certificate",
                "tags": [
                  "1.0.1",
                  "1.1.1",
                  "1.1.2",
                  "2.1.1",
                  "3.0.1",
                  "3.0.2",
                  "3.1.1",
                  "3.2.1",
                  "3.3.1",
                  "3.4.1",
                  "3.4.2"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/create-kv-certificate/1.0.1/modules/deployment-scripts/create-kv-certificate/README.md"
                  },
                  "1.1.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/create-kv-certificate/1.1.1/modules/deployment-scripts/create-kv-certificate/README.md"
                  },
                  "1.1.2": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/create-kv-certificate/1.1.2/modules/deployment-scripts/create-kv-certificate/README.md"
                  },
                  "2.1.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/create-kv-certificate/2.1.1/modules/deployment-scripts/create-kv-certificate/README.md"
                  },
                  "3.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/create-kv-certificate/3.0.1/modules/deployment-scripts/create-kv-certificate/README.md"
                  },
                  "3.0.2": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/create-kv-certificate/3.0.2/modules/deployment-scripts/create-kv-certificate/README.md"
                  },
                  "3.1.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/create-kv-certificate/3.1.1/modules/deployment-scripts/create-kv-certificate/README.md"
                  },
                  "3.2.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/create-kv-certificate/3.2.1/modules/deployment-scripts/create-kv-certificate/README.md"
                  },
                  "3.3.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/create-kv-certificate/3.3.1/modules/deployment-scripts/create-kv-certificate/README.md"
                  },
                  "3.4.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/create-kv-certificate/3.4.1/modules/deployment-scripts/create-kv-certificate/README.md"
                  },
                  "3.4.2": {
                    "description": "Create Key Vault self-signed certificates. Requires Key Vaults to be using RBAC Authorization, not Access Policies.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/create-kv-certificate/3.4.2/modules/deployment-scripts/create-kv-certificate/README.md"
                  }
                }
              },
              {
                "moduleName": "deployment-scripts/create-kv-sshkeypair",
                "tags": [
                  "1.0.1",
                  "1.0.2"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/create-kv-sshkeypair/1.0.1/modules/deployment-scripts/create-kv-sshkeypair/README.md"
                  },
                  "1.0.2": {
                    "description": "Creates SSH Key Pair Stores them in KeyVault as Secrets",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/create-kv-sshkeypair/1.0.2/modules/deployment-scripts/create-kv-sshkeypair/README.md"
                  }
                }
              },
              {
                "moduleName": "deployment-scripts/import-acr",
                "tags": [
                  "1.0.1",
                  "2.0.1",
                  "2.1.1",
                  "3.0.1",
                  "3.0.2"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/import-acr/1.0.1/modules/deployment-scripts/import-acr/README.md"
                  },
                  "2.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/import-acr/2.0.1/modules/deployment-scripts/import-acr/README.md"
                  },
                  "2.1.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/import-acr/2.1.1/modules/deployment-scripts/import-acr/README.md"
                  },
                  "3.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/import-acr/3.0.1/modules/deployment-scripts/import-acr/README.md"
                  },
                  "3.0.2": {
                    "description": "An Azure CLI Deployment Script that imports public container images to an Azure Container Registry",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/import-acr/3.0.2/modules/deployment-scripts/import-acr/README.md"
                  }
                }
              },
              {
                "moduleName": "deployment-scripts/wait",
                "tags": [
                  "1.0.1",
                  "1.0.2",
                  "1.1.1"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/wait/1.0.1/modules/deployment-scripts/wait/README.md"
                  },
                  "1.0.2": {
                    "description": "A Deployment Script that introduces a delay to the deployment process.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/wait/1.0.2/modules/deployment-scripts/wait/README.md"
                  },
                  "1.1.1": {
                    "description": "A Deployment Script that introduces a delay to the deployment process.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/deployment-scripts/wait/1.1.1/modules/deployment-scripts/wait/README.md"
                  }
                }
              },
              {
                "moduleName": "identity/user-assigned-identity",
                "tags": [
                  "1.0.1",
                  "1.0.2"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/identity/user-assigned-identity/1.0.1/modules/identity/user-assigned-identity/README.md"
                  },
                  "1.0.2": {
                    "description": "Azure managed identities provide an easy way for applications to access resources securely in Azure.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/identity/user-assigned-identity/1.0.2/modules/identity/user-assigned-identity/README.md"
                  }
                }
              },
              {
                "moduleName": "lz/sub-vending",
                "tags": [
                  "1.1.1",
                  "1.1.2",
                  "1.2.1",
                  "1.2.2",
                  "1.3.1",
                  "1.4.1",
                  "1.4.2"
                ],
                "properties": {
                  "1.1.1": {
                    "description": "v1.1.1: These are the input parameters for the Bicep module: [`main.bicep`](./main.bicep)\n\nThis is the orchestration module that is used and called by a consumer of the module to deploy a Landing Zone Subscription and its associated resources, based on the parameter input values that are provided to it at deployment time.\n\n> For more information and examples please see the [wiki](https://github.com/Azure/bicep-lz-vending/wiki)",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/lz/sub-vending/1.1.1/modules/lz/sub-vending/README.md"
                  },
                  "1.1.2": {
                    "description": "v1.1.2: These are the input parameters for the Bicep module: [`main.bicep`](./main.bicep)\n\nThis is the orchestration module that is used and called by a consumer of the module to deploy a Landing Zone Subscription and its associated resources, based on the parameter input values that are provided to it at deployment time.\n\n> For more information and examples please see the [wiki](https://github.com/Azure/bicep-lz-vending/wiki)",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/lz/sub-vending/1.1.2/modules/lz/sub-vending/README.md"
                  },
                  "1.2.1": {
                    "description": "v1.2.1: These are the input parameters for the Bicep module: [`main.bicep`](./main.bicep)\n\nThis is the orchestration module that is used and called by a consumer of the module to deploy a Landing Zone Subscription and its associated resources, based on the parameter input values that are provided to it at deployment time.\n\n> For more information and examples please see the [wiki](https://github.com/Azure/bicep-lz-vending/wiki)",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/lz/sub-vending/1.2.1/modules/lz/sub-vending/README.md"
                  },
                  "1.2.2": {
                    "description": "v1.2.2: These are the input parameters for the Bicep module: [`main.bicep`](./main.bicep)\n\nThis is the orchestration module that is used and called by a consumer of the module to deploy a Landing Zone Subscription and its associated resources, based on the parameter input values that are provided to it at deployment time.\n\n> For more information and examples please see the [wiki](https://github.com/Azure/bicep-lz-vending/wiki)",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/lz/sub-vending/1.2.2/modules/lz/sub-vending/README.md"
                  },
                  "1.3.1": {
                    "description": "v1.3.1: These are the input parameters for the Bicep module: [`main.bicep`](./main.bicep)\n\nThis is the orchestration module that is used and called by a consumer of the module to deploy a Landing Zone Subscription and its associated resources, based on the parameter input values that are provided to it at deployment time.\n\n> For more information and examples please see the [wiki](https://github.com/Azure/bicep-lz-vending/wiki)",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/lz/sub-vending/1.3.1/modules/lz/sub-vending/README.md"
                  },
                  "1.4.1": {
                    "description": "v1.4.1: This module is designed to accelerate deployment of landing zones (aka Subscriptions) within an Azure AD Tenant.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/lz/sub-vending/1.4.1/modules/lz/sub-vending/README.md"
                  },
                  "1.4.2": {
                    "description": "v1.4.2: This module is designed to accelerate deployment of landing zones (aka Subscriptions) within an Azure AD Tenant.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/lz/sub-vending/1.4.2/modules/lz/sub-vending/README.md"
                  }
                }
              },
              {
                "moduleName": "network/dns-zone",
                "tags": [
                  "1.0.1",
                  "1.0.4"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/network/dns-zone/1.0.1/modules/network/dns-zone/README.md"
                  },
                  "1.0.4": {
                    "description": "Azure DNS is a hosting service for DNS domains that provides name resolution.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/network/dns-zone/1.0.4/modules/network/dns-zone/README.md"
                  }
                }
              },
              {
                "moduleName": "network/nat-gateway",
                "tags": [
                  "1.0.1",
                  "1.0.4"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/network/nat-gateway/1.0.1/modules/network/nat-gateway/README.md"
                  },
                  "1.0.4": {
                    "description": "A bicep module for simplified deployment for NAT gateways and available configuration options.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/network/nat-gateway/1.0.4/modules/network/nat-gateway/README.md"
                  }
                }
              },
              {
                "moduleName": "network/private-dns-zone",
                "tags": [
                  "1.0.1"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/network/private-dns-zone/1.0.1/modules/network/private-dns-zone/README.md"
                  }
                }
              },
              {
                "moduleName": "network/public-ip-address",
                "tags": [
                  "1.0.1",
                  "1.0.2"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/network/public-ip-address/1.0.1/modules/network/public-ip-address/README.md"
                  },
                  "1.0.2": {
                    "description": "Bicep Module for creating Public Ip Address",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/network/public-ip-address/1.0.2/modules/network/public-ip-address/README.md"
                  }
                }
              },
              {
                "moduleName": "network/public-ip-prefix",
                "tags": [
                  "1.0.1",
                  "1.0.2"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/network/public-ip-prefix/1.0.1/modules/network/public-ip-prefix/README.md"
                  },
                  "1.0.2": {
                    "description": "Bicep Module for creating Public Ip Prefix",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/network/public-ip-prefix/1.0.2/modules/network/public-ip-prefix/README.md"
                  }
                }
              },
              {
                "moduleName": "network/traffic-manager",
                "tags": [
                  "1.0.1",
                  "2.0.1",
                  "2.1.1",
                  "2.2.1",
                  "2.3.1",
                  "2.3.2",
                  "2.3.3"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/network/traffic-manager/1.0.1/modules/network/traffic-manager/README.md"
                  },
                  "2.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/network/traffic-manager/2.0.1/modules/network/traffic-manager/README.md"
                  },
                  "2.1.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/network/traffic-manager/2.1.1/modules/network/traffic-manager/README.md"
                  },
                  "2.2.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/network/traffic-manager/2.2.1/modules/network/traffic-manager/README.md"
                  },
                  "2.3.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/network/traffic-manager/2.3.1/modules/network/traffic-manager/README.md"
                  },
                  "2.3.2": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/network/traffic-manager/2.3.2/modules/network/traffic-manager/README.md"
                  },
                  "2.3.3": {
                    "description": "Bicep module for creating a Azure Traffic Manager Profile with endpoints and monitor configurations.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/network/traffic-manager/2.3.3/modules/network/traffic-manager/README.md"
                  }
                }
              },
              {
                "moduleName": "network/virtual-network",
                "tags": [
                  "1.0.1",
                  "1.0.2",
                  "1.0.3",
                  "1.1.1",
                  "1.1.2",
                  "1.1.3"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/network/virtual-network/1.0.1/modules/network/virtual-network/README.md"
                  },
                  "1.0.2": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/network/virtual-network/1.0.2/modules/network/virtual-network/README.md"
                  },
                  "1.0.3": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/network/virtual-network/1.0.3/modules/network/virtual-network/README.md"
                  },
                  "1.1.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/network/virtual-network/1.1.1/modules/network/virtual-network/README.md"
                  },
                  "1.1.2": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/network/virtual-network/1.1.2/modules/network/virtual-network/README.md"
                  },
                  "1.1.3": {
                    "description": "This module deploys Microsoft.Network Virtual Networks and optionally available children or extensions",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/network/virtual-network/1.1.3/modules/network/virtual-network/README.md"
                  }
                }
              },
              {
                "moduleName": "observability/grafana",
                "tags": [
                  "1.0.1",
                  "1.0.2",
                  "1.0.3"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/observability/grafana/1.0.1/modules/observability/grafana/README.md"
                  },
                  "1.0.2": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/observability/grafana/1.0.2/modules/observability/grafana/README.md"
                  },
                  "1.0.3": {
                    "description": "Azure Managed Grafana is a data visualization platform built on top of the Grafana software by Grafana Labs.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/observability/grafana/1.0.3/modules/observability/grafana/README.md"
                  }
                }
              },
              {
                "moduleName": "samples/array-loop",
                "$comment": "Tags intentionally out of order",
                "tags": [
                  "1.0.1",
                  "1.10.1",
                  "1.0.2",
                  "1.0.2-preview",
                  "1.0.3"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/samples/array-loop/1.0.1/modules/samples/array-loop/README.md"
                  },
                  "1.0.2": {
                    "description": "v1.0.1: A sample Bicep registry module demonstrating array iterations.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/samples/array-loop/1.0.2/modules/samples/array-loop/README.md"
                  },
                  "1.0.3": {
                    "description": "v1.0.3: A sample Bicep registry module demonstrating array iterations.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/samples/array-loop/1.0.3/modules/samples/array-loop/README.md"
                  }
                }
              },
              {
                "moduleName": "samples/hello-world",
                "tags": [
                  "1.0.1",
                  "1.0.2",
                  "1.0.3",
                  "1.0.4"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/samples/hello-world/1.0.1/modules/samples/hello-world/README.md"
                  },
                  "1.0.2": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/samples/hello-world/1.0.2/modules/samples/hello-world/README.md"
                  },
                  "1.0.3": {
                    "description": "A \"שָׁלוֹם עוֹלָם\" sample Bicep registry module",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/samples/hello-world/1.0.3/modules/samples/hello-world/README.md"
                  },
                  "1.0.4": {
                    "description": "A \"שָׁלוֹם עוֹלָם\" sample Bicep registry module",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/samples/hello-world/1.0.4/modules/samples/hello-world/README.md"
                  }
                }
              },
              {
                "moduleName": "search/search-service",
                "tags": [
                  "1.0.1",
                  "1.0.2"
                ],
                "properties": {
                  "1.0.1": {
                    "description": "Bicep module for simplified deployment of Azure Cognitive Search.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/search/search-service/1.0.1/modules/search/search-service/README.md"
                  },
                  "1.0.2": {
                    "description": "Bicep module for simplified deployment of Azure Cognitive Search.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/search/search-service/1.0.2/modules/search/search-service/README.md"
                  }
                }
              },
              {
                "moduleName": "security/keyvault",
                "tags": [
                  "1.0.1",
                  "1.0.2"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/security/keyvault/1.0.1/modules/security/keyvault/README.md"
                  },
                  "1.0.2": {
                    "description": "Bicep module for simplified deployment of KeyVault; enables VNet integration and offers flexible configuration options.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/security/keyvault/1.0.2/modules/security/keyvault/README.md"
                  }
                }
              },
              {
                "moduleName": "storage/cosmos-db",
                "tags": [
                  "1.0.1",
                  "2.0.1",
                  "3.0.1",
                  "3.0.2"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/storage/cosmos-db/1.0.1/modules/storage/cosmos-db/README.md"
                  },
                  "2.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/storage/cosmos-db/2.0.1/modules/storage/cosmos-db/README.md"
                  },
                  "3.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/storage/cosmos-db/3.0.1/modules/storage/cosmos-db/README.md"
                  },
                  "3.0.2": {
                    "description": "Bicep module for simplified deployment of Cosmos DB; enables VNet integration and offers flexible configuration options.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/storage/cosmos-db/3.0.2/modules/storage/cosmos-db/README.md"
                  }
                }
              },
              {
                "moduleName": "storage/data-explorer",
                "tags": [
                  "0.0.1"
                ],
                "properties": {
                  "0.0.1": {
                    "description": "This Bicep module creates a Kusto Cluster with the specified number of nodes and version.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/storage/data-explorer/0.0.1/modules/storage/data-explorer/README.md"
                  }
                }
              },
              {
                "moduleName": "storage/log-analytics-workspace",
                "tags": [
                  "1.0.1",
                  "1.0.2",
                  "1.0.3"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/storage/log-analytics-workspace/1.0.1/modules/storage/log-analytics-workspace/README.md"
                  },
                  "1.0.2": {
                    "description": "This module deploys Log Analytics workspace and optionally available integrations.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/storage/log-analytics-workspace/1.0.2/modules/storage/log-analytics-workspace/README.md"
                  },
                  "1.0.3": {
                    "description": "This module deploys Log Analytics workspace and optionally available integrations.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/storage/log-analytics-workspace/1.0.3/modules/storage/log-analytics-workspace/README.md"
                  }
                }
              },
              {
                "moduleName": "storage/mysql-single-server",
                "tags": [
                  "1.0.1",
                  "1.0.2",
                  "1.0.3"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/storage/mysql-single-server/1.0.1/modules/storage/mysql-single-server/README.md"
                  },
                  "1.0.2": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/storage/mysql-single-server/1.0.2/modules/storage/mysql-single-server/README.md"
                  },
                  "1.0.3": {
                    "description": "This Bicep Module is used for the deployment of MySQL DB  Single Server with reusable set of parameters and resources.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/storage/mysql-single-server/1.0.3/modules/storage/mysql-single-server/README.md"
                  }
                }
              },
              {
                "moduleName": "storage/postgresql-single-server",
                "tags": [
                  "1.0.1",
                  "1.1.1",
                  "1.1.2"
                ],
                "properties": {
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/storage/postgresql-single-server/1.0.1/modules/storage/postgresql-single-server/README.md"
                  },
                  "1.1.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/storage/postgresql-single-server/1.1.1/modules/storage/postgresql-single-server/README.md"
                  },
                  "1.1.2": {
                    "description": "This module deploys PostgreSQL Single Server (Microsoft.DBforPostgreSQL/servers) and optionally available integrations.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/storage/postgresql-single-server/1.1.2/modules/storage/postgresql-single-server/README.md"
                  }
                }
              },
              {
                "moduleName": "storage/redis-cache",
                "tags": [
                  "0.0.1",
                  "1.0.1",
                  "2.0.1"
                ],
                "properties": {
                  "0.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/storage/redis-cache/0.0.1/modules/storage/redis-cache/README.md"
                  },
                  "1.0.1": {
                    "description": "This module deploys Azure Cache for Redis(Microsoft.Cache/redis) and optionally available integrations.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/storage/redis-cache/1.0.1/modules/storage/redis-cache/README.md"
                  },
                  "2.0.1": {
                    "description": "This module deploys Azure Cache for Redis(Microsoft.Cache/redis) and optionally available integrations.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/storage/redis-cache/2.0.1/modules/storage/redis-cache/README.md"
                  }
                }
              },
              {
                "moduleName": "storage/storage-account",
                "tags": [
                  "0.0.1",
                  "1.0.1",
                  "2.0.1",
                  "2.0.2",
                  "2.0.3",
                  "3.0.1"
                ],
                "properties": {
                  "0.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/storage/storage-account/0.0.1/modules/storage/storage-account/README.md"
                  },
                  "1.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/storage/storage-account/1.0.1/modules/storage/storage-account/README.md"
                  },
                  "2.0.1": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/storage/storage-account/2.0.1/modules/storage/storage-account/README.md"
                  },
                  "2.0.2": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/storage/storage-account/2.0.2/modules/storage/storage-account/README.md"
                  },
                  "2.0.3": {
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/storage/storage-account/2.0.3/modules/storage/storage-account/README.md"
                  },
                  "3.0.1": {
                    "description": "This Bicep module creates a Storage Account with zone-redundancy, encryption, virtual network access, and TLS version.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/storage/storage-account/3.0.1/modules/storage/storage-account/README.md"
                  }
                }
              },
              {
                "moduleName": "avm-res-compute-sshpublickey",
                "tags": [
                  "0.1.0"
                ],
                "properties": {
                  "0.1.0": {
                    "description": "This module deploys a Public SSH Key.\n\n> Note: The resource does not auto-generate the key for you.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/avm/res/compute/ssh-public-key/0.1.0/avm/res/compute/ssh-public-key/README.md"
                  }
                }
              },
              {
                "moduleName": "avm-res-insights-actiongroup",
                "tags": [
                  "0.1.0"
                ],
                "properties": {
                  "0.1.0": {
                    "description": "This module deploys an Action Group.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/avm/res/insights/action-group/0.1.0/avm/res/insights/action-group/README.md"
                  }
                }
              },
              {
                "moduleName": "avm-res-keyvault-vault",
                "tags": [
                  "0.1.0"
                ],
                "properties": {
                  "0.1.0": {
                    "description": "This module deploys a Key Vault.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/avm/res/key-vault/vault/0.1.0/avm/res/key-vault/vault/README.md"
                  }
                }
              },
              {
                "moduleName": "avm-res-kubernetesconfiguration-extension",
                "tags": [
                  "0.1.0"
                ],
                "properties": {
                  "0.1.0": {
                    "description": "This module deploys a Kubernetes Configuration Extension.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/avm/res/kubernetes-configuration/extension/0.1.0/avm/res/kubernetes-configuration/extension/README.md"
                  }
                }
              },
              {
                "moduleName": "avm-res-kubernetesconfiguration-fluxconfiguration",
                "tags": [
                  "0.1.0",
                  "0.1.1"
                ],
                "properties": {
                  "0.1.0": {
                    "description": "This module deploys a Kubernetes Configuration Flux Configuration.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/avm/res/kubernetes-configuration/flux-configuration/0.1.0/avm/res/kubernetes-configuration/flux-configuration/README.md"
                  },
                  "0.1.1": {
                    "description": "This module deploys a Kubernetes Configuration Flux Configuration.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/avm/res/kubernetes-configuration/flux-configuration/0.1.1/avm/res/kubernetes-configuration/flux-configuration/README.md"
                  }
                }
              },
              {
                "moduleName": "avm-res-network-privateendpoint",
                "tags": [
                  "0.1.0",
                  "0.1.1"
                ],
                "properties": {
                  "0.1.0": {
                    "description": "This module deploys a Private Endpoint.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/avm/res/network/private-endpoint/0.1.0/avm/res/network/private-endpoint/README.md"
                  },
                  "0.1.1": {
                    "description": "This module deploys a Private Endpoint.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/avm/res/network/private-endpoint/0.1.1/avm/res/network/private-endpoint/README.md"
                  }
                }
              },
              {
                "moduleName": "avm-res-network-publicipaddress",
                "tags": [
                  "0.1.0"
                ],
                "properties": {
                  "0.1.0": {
                    "description": "This module deploys a Public IP Address.",
                    "documentationUri": "https://github.com/Azure/bicep-registry-modules/tree/avm/res/network/public-ip-address/0.1.0/avm/res/network/public-ip-address/README.md"
                  }
                }
              }
            ]
            """;

        private static readonly MockHttpMessageHandler MockHttpMessageHandler = new();

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            MockHttpMessageHandler
                .When(HttpMethod.Get, "*")
                .Respond("application/json", ModuleIndexJson);
        }

        private record ModuleMetadata_Original(string moduleName, List<string> tags);

        [TestMethod]
        public void GetModules_ForwardsCompatibleWithOriginalVersion()
        {
            // Earlier Bicep versions should not be confused by new metadata formats
            var metadataStream = new MemoryStream(Encoding.UTF8.GetBytes(ModuleIndexJson));
            ModuleMetadata_Original[] metadata = [.. JsonSerializer.Deserialize<ModuleMetadata_Original[]>(metadataStream)!];

            metadata.Length.Should().BeGreaterThanOrEqualTo(29);
            metadata.Select(m => m.moduleName).Should().Contain("samples/array-loop");
            metadata.First(m => m.moduleName == "samples/array-loop").tags.Should().Contain("1.0.1");
            metadata.First(m => m.moduleName == "samples/array-loop").tags.Should().Contain("1.0.2");
        }

        [TestMethod]
        public async Task GetModules_Count_SanityCheck()
        {
            PublicModuleMetadataProvider provider = new(CreateTypedClient());
            await provider.TryAwaitCache(false);
            var modules = await provider.TryGetModulesAsync();
            modules.Should().HaveCount(50);
        }

        [TestMethod]
        public async Task GetModules_IfOnlyLastTagHasDescription()
        {
            PublicModuleMetadataProvider provider = new(CreateTypedClient());
            await provider.TryAwaitCache(false);
            var modules = await provider.TryGetModulesAsync();
            var m = modules.Should().Contain(m => m.ModuleName == "bicep/samples/hello-world")
                .Which;
            var details = await m.TryGetDetailsAsync();
            details.Description.Should().Be("A \"שָׁלוֹם עוֹלָם\" sample Bicep registry module");
            details.DocumentationUri.Should().Be("https://github.com/Azure/bicep-registry-modules/tree/samples/hello-world/1.0.4/modules/samples/hello-world/README.md");
        }

        [TestMethod]
        public async Task GetModules_IfMultipleTagsHaveDescriptions()
        {
            PublicModuleMetadataProvider provider = new(CreateTypedClient());
            await provider.TryAwaitCache(false);
            var modules = await provider.TryGetModulesAsync();
            var m = modules.Should().Contain(m => m.ModuleName == "bicep/lz/sub-vending")
                .Which;
            var details = await m.TryGetDetailsAsync();
            details.Description.Should().Be("v1.4.2: This module is designed to accelerate deployment of landing zones (aka Subscriptions) within an Azure AD Tenant.");
            details.DocumentationUri.Should().Be("https://github.com/Azure/bicep-registry-modules/tree/lz/sub-vending/1.4.2/modules/lz/sub-vending/README.md");
        }

        [TestMethod]
        public async Task GetModuleVersions_SortsBySemver()
        {
            PublicModuleMetadataProvider provider = new(CreateTypedClient());
            var versions = await (await provider.TryGetModuleAsync("bicep/samples/array-loop"))!.TryGetVersionsAsync();

            versions.Select(v => v.Version).Should().Equal(
                "1.0.1",
                "1.0.2-preview",
                "1.0.2",
                "1.0.3",
                "1.10.1");
        }
    }
}
