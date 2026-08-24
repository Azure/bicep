// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Net;
using System.ServiceProcess;
using Bicep.Core.Features;
using Bicep.Core.Registry.Catalog;
using Bicep.Core.Tracing;
using Bicep.Core.Utils;
using Bicep.LanguageServer.Features.Custom.Build;
using Bicep.LanguageServer.Features.Custom.Configuration;
using Bicep.LanguageServer.Features.Custom.Decompile;
using Bicep.LanguageServer.Features.Custom.Deployments;
using Bicep.LanguageServer.Features.Custom.ImportKubernetesManifest;
using Bicep.LanguageServer.Features.Custom.InsertResource;
using Bicep.LanguageServer.Features.Custom.Linter;
using Bicep.LanguageServer.Features.Custom.LocalDeploy;
using Bicep.LanguageServer.Features.Custom.ModuleRestore;
using Bicep.LanguageServer.Features.Custom.Parameters;
using Bicep.LanguageServer.Features.Custom.Visualization;
using Bicep.LanguageServer.Features.Language.CodeAction;
using Bicep.LanguageServer.Features.Language.CodeLens;
using Bicep.LanguageServer.Features.Language.Completion;
using Bicep.LanguageServer.Features.Language.Definition;
using Bicep.LanguageServer.Features.Language.DocumentFormatting;
using Bicep.LanguageServer.Features.Language.DocumentHighlight;
using Bicep.LanguageServer.Features.Language.DocumentLink;
using Bicep.LanguageServer.Features.Language.DocumentSymbol;
using Bicep.LanguageServer.Features.Language.Hover;
using Bicep.LanguageServer.Features.Language.PrepareRename;
using Bicep.LanguageServer.Features.Language.References;
using Bicep.LanguageServer.Features.Language.Rename;
using Bicep.LanguageServer.Features.Language.SemanticTokens;
using Bicep.LanguageServer.Features.Language.SignatureHelp;
using Bicep.LanguageServer.Features.Language.TextDocumentSync;
using Bicep.LanguageServer.Features.Workspace.DidChangeConfiguration;
using Bicep.LanguageServer.Features.Workspace.DidChangeWatchedFiles;
using Bicep.LanguageServer.Options;
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
        private readonly IEnvironment environment;
        public Server(BicepLangServerOptions bicepLangServerOptions, Action<LanguageServerOptions> onOptionsFunc)
        {
            environment = new Core.Utils.Environment();
            server = OmnisharpLanguageServer.PreInit(options =>
            {
                options
                    // Language features
                    .WithHandler<BicepCodeActionHandler>()
                    .WithHandler<BicepCodeLensHandler>()
                    .WithHandler<BicepCompletionHandler>()
                    .WithHandler<BicepDefinitionHandler>()
                    .WithHandler<BicepDocumentFormattingHandler>()
                    .WithHandler<BicepDocumentHighlightHandler>()
                    .WithHandler<BicepExternalSourceDocumentLinkHandler>()
                    .WithHandler<BicepExternalSourceRequestHandler>()
                    .WithHandler<BicepDocumentSymbolHandler>()
                    .WithHandler<BicepHoverHandler>()
                    .WithHandler<BicepPrepareRenameHandler>()
                    .WithHandler<BicepReferencesHandler>()
                    .WithHandler<BicepRenameHandler>()
                    .WithHandler<BicepSemanticTokensHandler>()
                    .WithHandler<BicepSignatureHelpHandler>()
                    .WithHandler<BicepTextDocumentSyncHandler>()

                    // Workspace features
                    .WithHandler<BicepDidChangeConfigurationHandler>()
                    .WithHandler<BicepDidChangeWatchedFilesHandler>()

                    // Custom features
                    .WithHandler<BicepBuildCommandHandler>()
                    .WithHandler<BicepBuildParamsCommandHandler>()
                    .WithHandler<BicepCreateConfigFileHandler>()
                    .WithHandler<BicepGetRecommendedConfigLocationHandler>()
                    .WithHandler<BicepDecompileCommandHandler>()
                    .WithHandler<BicepDecompileSaveCommandHandler>()
                    .WithHandler<BicepDecompileForPasteCommandHandler>()
                    .WithHandler<BicepDecompileParamsCommandHandler>()
                    .WithHandler<GetDeploymentDataHandler>()
                    .WithHandler<BicepDeploymentStartCommandHandler>()
                    // Base handler (ExecuteTypedResponseCommandHandlerBase) is serial. This blocks other commands on the client side.
                    // To avoid the above issue, we'll change the RequestProcessType to parallel
                    .WithHandler<BicepDeploymentWaitForCompletionCommandHandler>(new JsonRpcHandlerOptions() { RequestProcessType = RequestProcessType.Parallel })
                    .WithHandler<BicepDeploymentScopeRequestHandler>()
                    .WithHandler<BicepDeploymentParametersHandler>()
                    .WithHandler<ImportKubernetesManifestHandler>()
                    .WithHandler<InsertResourceHandler>()
                    .WithHandler<BicepEditLinterRuleCommandHandler>()
                    .WithHandler<LocalDeployHandler>()
                    .WithHandler<BicepForceModulesRestoreCommandHandler>()
                    .WithHandler<BicepGenerateParamsCommandHandler>()
                    .WithHandler<VisualGraphUpdateHandler>()
                    .WithHandler<VisualGraphLayoutHandler>()
                    .WithHandler<VisualGraphNodeSourceHandler>()
                    .WithHandler<VisualResourceTypesHandler>()
                    .WithHandler<PrepareVisualResourceHandler>()
                    .WithServices(services => services.AddServerDependencies(bicepLangServerOptions));

                onOptionsFunc(options);
            });
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            await server.Initialize(cancellationToken);

            server.LogInfo($"Bicep version: {environment.GetVersionString()}, OS: {environment.CurrentPlatform?.ToString() ?? "unknown"}, Architecture: {environment.CurrentArchitecture}");
            server.LogInfo($"Running on processId {System.Environment.ProcessId}");

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

            var publicModuleMetadataProvider = server.GetRequiredService<IPublicModuleMetadataProvider>();
            publicModuleMetadataProvider.StartCache();
        }

        public void Dispose()
        {
            server.Dispose();
        }
    }
}
