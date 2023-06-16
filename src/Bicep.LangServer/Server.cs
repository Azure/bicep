// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Features;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Tracing;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Configuration;
using Bicep.LanguageServer.Deploy;
using Bicep.LanguageServer.Handlers;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Registry;
using Bicep.LanguageServer.Snippets;
using Bicep.LanguageServer.Telemetry;
using Bicep.LanguageServer.Utils;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Server;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using OmnisharpLanguageServer = OmniSharp.Extensions.LanguageServer.Server.LanguageServer;
using Bicep.LanguageServer.Settings;

namespace Bicep.LanguageServer
{
    public class Server : IDisposable
    {
        private readonly OmnisharpLanguageServer server;

        public Server(Action<LanguageServerOptions> onOptionsFunc)
        {
            server = OmnisharpLanguageServer.PreInit(options =>
            {
                options
                    .WithHandler<BicepTextDocumentSyncHandler>()
                    .WithHandler<BicepDocumentSymbolHandler>()
                    .WithHandler<BicepDefinitionHandler>()
                    .WithHandler<BicepDeploymentGraphHandler>()
                    .WithHandler<BicepReferencesHandler>()
                    .WithHandler<BicepDocumentHighlightHandler>()
                    .WithHandler<BicepDocumentFormattingHandler>()
                    .WithHandler<BicepRenameHandler>()
                    .WithHandler<BicepHoverHandler>()
                    .WithHandler<BicepCompletionHandler>()
                    .WithHandler<BicepCodeActionHandler>()
                    .WithHandler<BicepCreateConfigFileHandler>()
                    .WithHandler<BicepDidChangeWatchedFilesHandler>()
                    .WithHandler<BicepEditLinterRuleCommandHandler>()
                    .WithHandler<BicepGetRecommendedConfigLocationHandler>()
                    .WithHandler<BicepSignatureHelpHandler>()
                    .WithHandler<BicepSemanticTokensHandler>()
                    .WithHandler<BicepTelemetryHandler>()
                    .WithHandler<BicepBuildCommandHandler>()
                    .WithHandler<BicepGenerateParamsCommandHandler>()
                    .WithHandler<BicepBuildParamsCommandHandler>()
                    .WithHandler<BicepDeploymentStartCommandHandler>()
                    // Base handler (ExecuteTypedResponseCommandHandlerBase) is serial. This blocks other commands on the client side.
                    // To avoid the above issue, we'll change the RequestProcessType to parallel
                    .WithHandler<BicepDeploymentWaitForCompletionCommandHandler>(new JsonRpcHandlerOptions() { RequestProcessType = RequestProcessType.Parallel })
                    .WithHandler<BicepDecompileCommandHandler>()
                    .WithHandler<BicepDecompileSaveCommandHandler>()
                    .WithHandler<BicepDecompileForPasteCommandHandler>()
                    .WithHandler<BicepDecompileParamsCommandHandler>()
                    .WithHandler<BicepDeploymentScopeRequestHandler>()
                    .WithHandler<BicepDeploymentParametersHandler>()
                    .WithHandler<ImportKubernetesManifestHandler>()
                    .WithHandler<BicepForceModulesRestoreCommandHandler>()
                    .WithHandler<BicepRegistryCacheRequestHandler>()
                    .WithHandler<InsertResourceHandler>()
                    .WithHandler<ConfigurationSettingsHandler>()
                    .WithServices(RegisterServices);

                onOptionsFunc(options);
            });
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            await server.Initialize(cancellationToken);

            server.LogInfo($"Running on processId {Environment.ProcessId}");

            if (FeatureProvider.TracingEnabled)
            {
                Trace.Listeners.Add(new ServerLogTraceListener(server));
            }

            using (FeatureProvider.TracingEnabled ? AzureEventSourceListenerFactory.Create(FeatureProvider.TracingVerbosity) : null)
            {
                var scheduler = server.GetRequiredService<IModuleRestoreScheduler>();
                scheduler.Start();

                await server.WaitForExit;
            }
        }

        private static void RegisterServices(IServiceCollection services)
        {
            // using type based registration so dependencies can be injected automatically
            // without manually constructing up the graph
            services
                .AddBicepCore()
                .AddBicepDecompiler()
                .AddBicepparamDecompiler()
                .AddSingleton<IWorkspace, Workspace>()
                .AddSingleton<ISnippetsProvider, SnippetsProvider>()
                .AddSingleton<ITelemetryProvider, TelemetryProvider>()
                .AddSingleton<ICompilationManager, BicepCompilationManager>()
                .AddSingleton<ICompilationProvider, BicepCompilationProvider>()
                .AddSingleton<ISymbolResolver, BicepSymbolResolver>()
                .AddSingleton<ICompletionProvider, BicepCompletionProvider>()
                .AddSingleton<IModuleRestoreScheduler, ModuleRestoreScheduler>()
                .AddSingleton<IAzResourceProvider, AzResourceProvider>()
                .AddSingleton<IBicepConfigChangeHandler, BicepConfigChangeHandler>()
                .AddSingleton<IDeploymentCollectionProvider, DeploymentCollectionProvider>()
                .AddSingleton<IDeploymentOperationsCache, DeploymentOperationsCache>()
                .AddSingleton<IDeploymentFileCompilationCache, DeploymentFileCompilationCache>()
                .AddSingleton<IClientCapabilitiesProvider, ClientCapabilitiesProvider>()
                .AddSingleton<IModuleReferenceCompletionProvider, ModuleReferenceCompletionProvider>()
                .AddSingleton<ITokenCredentialFactory, TokenCredentialFactory>()
                .AddSingleton<IArmClientProvider, ArmClientProvider>()
                .AddSingleton<ISettingsProvider, SettingsProvider>()
                .AddSingleton<IAzureContainerRegistriesProvider, AzureContainerRegistriesProvider>()
                .AddSingleton<IPublicRegistryModuleMetadataProvider>(sp => new PublicRegistryModuleMetadataProvider(@"[
  {
    ""moduleName"": ""app/dapr-containerapp"",
    ""tags"": [
      ""1.0.1"",
      ""1.0.2""
    ]
  },
  {
    ""moduleName"": ""app/dapr-containerapps-environment"",
    ""tags"": [
      ""1.0.1"",
      ""1.1.1"",
      ""1.2.1"",
      ""1.2.2""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""azure-gaming/game-dev-vm"",
    ""tags"": [
      ""1.0.1"",
      ""1.0.2"",
      ""2.0.1"",
      ""2.0.2""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""azure-gaming/game-dev-vmss"",
    ""tags"": [
      ""1.0.1"",
      ""1.1.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""compute/availability-set"",
    ""tags"": [
      ""1.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""compute/container-registry"",
    ""tags"": [
      ""1.0.1"",
      ""1.0.2""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""compute/custom-image-vmss"",
    ""tags"": [
      ""1.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""cost/resourcegroup-scheduled-action"",
    ""tags"": [
      ""1.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""cost/subscription-scheduled-action"",
    ""tags"": [
      ""1.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""deployment-scripts/aks-run-command"",
    ""tags"": [
      ""1.0.1"",
      ""1.0.2"",
      ""1.0.3"",
      ""2.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""deployment-scripts/aks-run-helm"",
    ""tags"": [
      ""1.0.1"",
      ""2.0.1"",
      ""2.0.2""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""deployment-scripts/build-acr"",
    ""tags"": [
      ""1.0.1"",
      ""1.0.2"",
      ""2.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""deployment-scripts/create-kv-certificate"",
    ""tags"": [
      ""1.0.1"",
      ""1.1.1"",
      ""1.1.2"",
      ""2.1.1"",
      ""3.0.1"",
      ""3.0.2"",
      ""3.1.1"",
      ""3.2.1"",
      ""3.3.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""deployment-scripts/import-acr"",
    ""tags"": [
      ""1.0.1"",
      ""2.0.1"",
      ""2.1.1"",
      ""3.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""deployment-scripts/wait"",
    ""tags"": [
      ""1.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""identity/user-assigned-identity"",
    ""tags"": [
      ""1.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""lz/sub-vending"",
    ""tags"": [
      ""1.1.1"",
      ""1.1.2"",
      ""1.2.1"",
      ""1.2.2"",
      ""1.3.1""
    ],
    ""properties"": {
      ""1.1.1"": {
        ""description"": ""1.1.1: These are the input parameters for the Bicep module: [`main.bicep`](./main.bicep)\n\nThis is the orchestration module that is used and called by a consumer of the module to deploy a Landing Zone Subscription and its associated resources, based on the parameter input values that are provided to it at deployment time.""
      },
      ""1.1.2"": {
        ""description"": ""1.1.2: These are the input parameters for the Bicep module: [`main.bicep`](./main.bicep)\n\nThis is the orchestration module that is used and called by a consumer of the module to deploy a Landing Zone Subscription and its associated resources, based on the parameter input values that are provided to it at deployment time.""
      },
      ""1.2.1"": {
        ""description"": ""1.2.1: These are the input parameters for the Bicep module: [`main.bicep`](./main.bicep)\n\nThis is the orchestration module that is used and called by a consumer of the module to deploy a Landing Zone Subscription and its associated resources, based on the parameter input values that are provided to it at deployment time.""
      },
      ""1.2.2"": {
        ""description"": ""1.2.2: These are the input parameters for the Bicep module: [`main.bicep`](./main.bicep)\n\nThis is the orchestration module that is used and called by a consumer of the module to deploy a Landing Zone Subscription and its associated resources, based on the parameter input values that are provided to it at deployment time.""
      },
      ""1.3.1"": {
        ""description"": ""1.3.1: These are the input parameters for the Bicep module: [`main.bicep`](./main.bicep)\n\nThis is the orchestration module that is used and called by a consumer of the module to deploy a Landing Zone Subscription and its associated resources, based on the parameter input values that are provided to it at deployment time.""
      }
    }
  },
  {
    ""moduleName"": ""network/dns-zone"",
    ""tags"": [
      ""1.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""network/nat-gateway"",
    ""tags"": [
      ""1.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""network/traffic-manager"",
    ""tags"": [
      ""1.0.1"",
      ""2.0.1"",
      ""2.1.1"",
      ""2.2.1"",
      ""2.3.1"",
      ""2.3.2""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""network/virtual-network"",
    ""tags"": [
      ""1.0.1"",
      ""1.0.2"",
      ""1.0.3"",
      ""1.1.1"",
      ""1.1.2""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""observability/grafana"",
    ""tags"": [
      ""1.0.1"",
      ""1.0.2""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""samples/array-loop"",
    ""tags"": [
      ""1.0.1"",
      ""1.0.2""
    ],
    ""properties"": {
      ""1.0.1"": {
        ""description"": ""Description for 1.0.1""
      }
    }
  },
  {
    ""moduleName"": ""samples/hello-world"",
    ""tags"": [
      ""1.0.1"",
      ""1.0.2"",
      ""1.0.3""
    ],
    ""properties"": {
      ""1.0.3"": {
        ""description"": ""A \""שָׁלוֹם עוֹלָם\"" sample Bicep registry module"",
        ""other property - we have to be forwards compatible"": ""whatever""
      },
      ""unexpected tag should be ignored"": {
      }
    },
    ""other property - we have to be forwards compatible"": ""whatever""
  },
  {
    ""moduleName"": ""security/keyvault"",
    ""tags"": [
      ""1.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""storage/cosmos-db"",
    ""tags"": [
      ""1.0.1"",
      ""2.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""storage/log-analytics-workspace"",
    ""tags"": [
      ""1.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""storage/redis-cache"",
    ""tags"": [
      ""0.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""storage/storage-account"",
    ""tags"": [
      ""0.0.1"",
      ""1.0.1"",
      ""2.0.1"",
      ""2.0.2""
    ],
    ""properties"": {}
  }
]"));
        }

        public void Dispose()
        {
            server.Dispose();
        }
    }
}
