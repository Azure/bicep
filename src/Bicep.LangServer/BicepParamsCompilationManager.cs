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
using System;
using Bicep.LanguageServer.Providers;
using Bicep.Core.Configuration;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.FileSystem;

namespace Bicep.LanguageServer
{
    public class BicepParamsCompilationManager : IParamsCompilationManager
    {   
        private readonly ILanguageServerFacade server;
        private readonly ICompilationProvider bicepCompilationContextProvider;
        private readonly IConfigurationManager bicepConfigurationManager;
        private readonly IFileResolver fileResolver;
        private readonly ConcurrentDictionary<DocumentUri, ParamsCompilationContext> activeContexts = new ConcurrentDictionary<DocumentUri, ParamsCompilationContext>();
        public BicepParamsCompilationManager(ILanguageServerFacade server, ICompilationProvider bicepCompilationContextProvider, IConfigurationManager bicepConfigurationManager, IFileResolver fileResolver)
        {
            this.server = server;
            this.bicepCompilationContextProvider = bicepCompilationContextProvider;
            this.bicepConfigurationManager = bicepConfigurationManager;
            this.fileResolver = fileResolver;
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
            var paramsFile = SourceFileFactory.CreateBicepParamFile(uri.ToUri(), text);

            Uri? bicepFileUri = ParamsSemanticModel.TryGetBicepFileUri(out var compilationLoadDiagnostics, fileResolver, paramsFile);

            ParamsSemanticModel semanticModel;

            if(bicepFileUri is {})
            {
                var bicepConfig = bicepConfigurationManager.GetConfiguration(bicepFileUri);
                var bicepCompilationContext = bicepCompilationContextProvider.Create(new Workspace(), bicepFileUri, new Dictionary<ISourceFile, ISemanticModel>().ToImmutableDictionary(), bicepConfig, new LinterAnalyzer(bicepConfig));
                semanticModel = new (paramsFile, compilationLoadDiagnostics, bicepCompilationContext.Compilation);
            }
            else
            {
                semanticModel = new (paramsFile, compilationLoadDiagnostics);
            }
    
            var context = this.activeContexts.AddOrUpdate(uri, 
            (uri) => new ParamsCompilationContext(semanticModel, semanticModel.BicepParamFile.ProgramSyntax, semanticModel.BicepParamFile.LineStarts), 
            (uri, prevContext) => new ParamsCompilationContext(semanticModel, semanticModel.BicepParamFile.ProgramSyntax, semanticModel.BicepParamFile.LineStarts));

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
                Diagnostics = new (diagnostics.ToDiagnostics(lineStarts))
            });
        }
    }
}
