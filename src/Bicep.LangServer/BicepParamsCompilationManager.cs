// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.LanguageServer.CompilationManager;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Bicep.LanguageServer.Extensions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.Providers;
using Bicep.Core.Configuration;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Features;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Semantics.Namespaces;

namespace Bicep.LanguageServer
{
    public class BicepParamsCompilationManager : IParamsCompilationManager
    {
        private readonly ILanguageServerFacade server;
        private readonly ICompilationProvider bicepCompilationContextProvider;
        private readonly IConfigurationManager bicepConfigurationManager;
        private readonly IFileResolver fileResolver;
        private readonly IModuleDispatcher moduleDispatcher;
        private readonly IWorkspace workspace;
        private readonly IFeatureProvider features;
        private readonly ApiVersionProvider apiVersionProvider;
        private readonly INamespaceProvider namespaceProvider;
        private readonly ConcurrentDictionary<DocumentUri, ParamsCompilationContext> activeContexts = new ConcurrentDictionary<DocumentUri, ParamsCompilationContext>();
        public BicepParamsCompilationManager(ILanguageServerFacade server, ICompilationProvider bicepCompilationContextProvider, IConfigurationManager bicepConfigurationManager, IFileResolver fileResolver, IModuleDispatcher moduleDispatcher, IWorkspace workspace, IFeatureProvider features, ApiVersionProvider apiVersionProvider, INamespaceProvider namespaceProvider)
        {
            this.server = server;
            this.bicepCompilationContextProvider = bicepCompilationContextProvider;
            this.bicepConfigurationManager = bicepConfigurationManager;
            this.fileResolver = fileResolver;
            this.moduleDispatcher = moduleDispatcher;
            this.workspace = workspace;
            this.features = features;
            this.apiVersionProvider = apiVersionProvider;
            this.namespaceProvider = namespaceProvider;
        }

        public void HandleFileChanges(IEnumerable<FileEvent> fileEvents)
        {
            //TODO: complete later, not required for basic file interaction
        }

        public void RefreshCompilation(DocumentUri uri, bool reloadBicepConfig = false)
        {
            //TODO: complete later, not required for basic file interaction
        }

        public void UpsertCompilation(DocumentUri uri, int? version, string text, string? languageId = null, bool triggeredByFileOpenEvent = false)
        {
            var inputUri = uri.ToUri();
            var configuration = bicepConfigurationManager.GetConfiguration(inputUri);

            var sourceFileGrouping = SourceFileGroupingBuilder.Build(this.fileResolver, this.moduleDispatcher, this.workspace, inputUri, configuration);

            var semanticModel = new ParamsSemanticModel(sourceFileGrouping, file => {
                var compilationGrouping = new SourceFileGrouping(fileResolver, file.FileUri, sourceFileGrouping.FileResultByUri, sourceFileGrouping.UriResultByModule, sourceFileGrouping.SourceFileParentLookup);


                return new Compilation(features, namespaceProvider, compilationGrouping, configuration, apiVersionProvider, new LinterAnalyzer(configuration));
            });

            var context = this.activeContexts.AddOrUpdate(
                uri,
                (uri) => new ParamsCompilationContext(semanticModel),
                (uri, prevContext) => new ParamsCompilationContext(semanticModel));

            this.PublishDocumentDiagnostics(uri, version, context.ParamsSemanticModel.GetAllDiagnostics(), context.LineStarts);
        }

        public void CloseCompilation(DocumentUri uri)
        {
            this.activeContexts.TryRemove(uri, out _);
        }

        public ParamsCompilationContext? GetCompilation(DocumentUri uri)
        {
            this.activeContexts.TryGetValue(uri, out var context);
            return context;
        }

        private void PublishDocumentDiagnostics(DocumentUri uri, int? version, IEnumerable<IDiagnostic> diagnostics, ImmutableArray<int> lineStarts)
        {
            server.TextDocument.PublishDiagnostics(new PublishDiagnosticsParams
            {
                Uri = uri,
                Version = version,
                Diagnostics = new(diagnostics.ToDiagnostics(lineStarts))
            });
        }
    }
}
