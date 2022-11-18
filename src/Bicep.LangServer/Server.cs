// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Configuration;
using Bicep.Core.Emit;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Tracing;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.Workspaces;
using Bicep.Decompiler;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Configuration;
using Bicep.LanguageServer.Deploy;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Handlers;
using Bicep.LanguageServer.ParamsHandlers;
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
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using OmnisharpLanguageServer = OmniSharp.Extensions.LanguageServer.Server.LanguageServer;

namespace Bicep.LanguageServer
{
    public class Server : IDisposable
    {
        private readonly OmnisharpLanguageServer server;

        public Server(Action<LanguageServerOptions> onOptionsFunc)
        {
            BicepDeploymentsInterop.Initialize();
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

                    .WithHandler<BicepParamsDefinitionHandler>()

                    .WithHandler<BicepTelemetryHandler>()
                    .WithHandler<BicepBuildCommandHandler>()
                    .WithHandler<BicepGenerateParamsCommandHandler>()
                    .WithHandler<BicepDeploymentStartCommandHandler>()
                    // Base handler (ExecuteTypedResponseCommandHandlerBase) is serial. This blocks other commands on the client side.
                    // To avoid the above issue, we'll change the RequestProcessType to parallel
                    .WithHandler<BicepDeploymentWaitForCompletionCommandHandler>(new JsonRpcHandlerOptions() { RequestProcessType = RequestProcessType.Parallel })
                    .WithHandler<BicepDecompileCommandHandler>()
                    .WithHandler<BicepDecompileSaveCommandHandler>()
                    .WithHandler<BicepDecompileForPasteCommandHandler>()
                    .WithHandler<BicepDeploymentScopeRequestHandler>()
                    .WithHandler<BicepDeploymentParametersHandler>()
                    .WithHandler<ImportKubernetesManifestHandler>()
                    .WithHandler<BicepForceModulesRestoreCommandHandler>()
                    .WithHandler<BicepRegistryCacheRequestHandler>()
                    .WithHandler<InsertResourceHandler>()
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
                .AddSingleton<IMcrCompletionProvider, McrCompletionProvider>()
                .AddSingleton<IModuleReferenceCompletionProvider, ModuleReferenceCompletionProvider>()
                .AddSingleton<IServiceClientCredentialsProvider, ServiceClientCredentialsProvider>()
                .AddSingleton<ITokenCredentialFactory, TokenCredentialFactory>();
        }

        public void Dispose()
        {
            server.Dispose();
        }
    }
}
