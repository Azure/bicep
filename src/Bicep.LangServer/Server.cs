// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Net;
using Bicep.Core.Features;
using Bicep.Core.Tracing;
using Bicep.LanguageServer.Handlers;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Registry;
using Bicep.LanguageServer.Settings;
using Bicep.LanguageServer.Utils;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Server;
using OmnisharpLanguageServer = OmniSharp.Extensions.LanguageServer.Server.LanguageServer;

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
                    .WithHandler<GetDeploymentDataHandler>()
                    .WithHandler<BicepReferencesHandler>()
                    .WithHandler<BicepExternalSourceDocumentLinkHandler>()
                    .WithHandler<BicepDocumentHighlightHandler>()
                    .WithHandler<BicepDocumentFormattingHandler>()
                    .WithHandler<BicepRenameHandler>()
                    .WithHandler<BicepHoverHandler>()
                    .WithHandler<BicepCompletionHandler>()
                    .WithHandler<BicepCodeActionHandler>()
                    .WithHandler<BicepCodeLensHandler>()
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
                    .WithHandler<BicepExternalSourceRequestHandler>()
                    .WithHandler<InsertResourceHandler>()
                    .WithHandler<ConfigurationSettingsHandler>()
                    .WithHandler<LocalDeployHandler>()
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

#pragma warning disable VSTHRD003 // Avoid awaiting foreign Tasks
                await server.WaitForExit;
#pragma warning restore VSTHRD003 // Avoid awaiting foreign Tasks
            }
        }

        private static void RegisterServices(IServiceCollection services)
        {
            // using type based registration for Http clients so dependencies can be injected automatically
            // without manually constructing up the graph, see https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory#typed-clients
            services.AddServerDependencies();

            services
                .AddHttpClient<IPublicRegistryModuleMetadataClient, PublicRegistryModuleMetadataClient>()
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                });
        }

        public void Dispose()
        {
            server.Dispose();
        }
    }
}
