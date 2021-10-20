// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Handlers;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Registry;
using Bicep.LanguageServer.Snippets;
using Bicep.LanguageServer.Telemetry;
using Bicep.LanguageServer.Utils;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Server;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using OmnisharpLanguageServer = OmniSharp.Extensions.LanguageServer.Server.LanguageServer;

namespace Bicep.LanguageServer
{
    public class Server
    {
        public record CreationOptions(
            ISnippetsProvider? SnippetsProvider = null,
            INamespaceProvider? NamespaceProvider = null,
            IFileResolver? FileResolver = null,
            IFeatureProvider? Features = null,
            IModuleRestoreScheduler? ModuleRestoreScheduler = null);

        private readonly OmnisharpLanguageServer server;

        public Server(PipeReader input, PipeWriter output, CreationOptions creationOptions)
            : this(creationOptions, options => options.WithInput(input).WithOutput(output))
        {
        }

        public Server(Stream input, Stream output, CreationOptions creationOptions)
            : this(creationOptions, options => options.WithInput(input).WithOutput(output))
        {
        }
        
        private Server(CreationOptions creationOptions, Action<LanguageServerOptions> onOptionsFunc)
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
                    .WithHandler<BicepDidChangeWatchedFilesHandler>()
                    .WithHandler<BicepDisableLinterRuleCommandHandler>()
                    .WithHandler<BicepSignatureHelpHandler>()
                    .WithHandler<BicepSemanticTokensHandler>()
                    .WithHandler<BicepTelemetryHandler>()
                    .WithHandler<BicepBuildCommandHandler>()
                    .WithHandler<BicepRegistryCacheRequestHandler>()
                    .WithServices(services => RegisterServices(creationOptions, services));

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

        private static void RegisterServices(CreationOptions creationOptions, IServiceCollection services)
        {
            // using type based registration so dependencies can be injected automatically
            // without manually constructing up the graph
            services.AddSingleton<IAzResourceTypeLoader, AzResourceTypeLoader>();
            services.AddSingletonOrInstance<INamespaceProvider, DefaultNamespaceProvider>(creationOptions.NamespaceProvider);
            services.AddSingletonOrInstance<ISnippetsProvider, SnippetsProvider>(creationOptions.SnippetsProvider);
            services.AddSingletonOrInstance<IFileResolver, FileResolver>(creationOptions.FileResolver);
            services.AddSingletonOrInstance<IFeatureProvider, FeatureProvider>(creationOptions.Features);
            services.AddSingleton<EmitterSettings>();
            services.AddSingleton<IModuleRegistryProvider, DefaultModuleRegistryProvider>();
            services.AddSingleton<IContainerRegistryClientFactory, ContainerRegistryClientFactory>();
            services.AddSingleton<ITemplateSpecRepositoryFactory, TemplateSpecRepositoryFactory>();
            services.AddSingleton<IModuleDispatcher, ModuleDispatcher>();
            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddSingleton<IConfigurationManager, ConfigurationManager>();
            services.AddSingleton<ITokenCredentialFactory, TokenCredentialFactory>();
            services.AddSingleton<ITelemetryProvider, TelemetryProvider>();
            services.AddSingleton<IWorkspace, Workspace>();
            services.AddSingleton<ICompilationManager, BicepCompilationManager>();
            services.AddSingleton<ICompilationProvider, BicepCompilationProvider>();
            services.AddSingleton<ISymbolResolver, BicepSymbolResolver>();
            services.AddSingleton<ICompletionProvider, BicepCompletionProvider>();
            services.AddSingletonOrInstance<IModuleRestoreScheduler, ModuleRestoreScheduler>(creationOptions.ModuleRestoreScheduler);
        }
    }
}
