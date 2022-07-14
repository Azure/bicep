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
using Bicep.Core.Syntax;
using Bicep.Core.FileSystem;
using System.Linq;
using System;
using Bicep.LanguageServer.Providers;
using Bicep.Core.Configuration;
using Bicep.Core.Analyzers.Linter;

namespace Bicep.LanguageServer
{
    public class BicepParamsCompilationManager : IParamsCompilationManager
    {   
        private readonly ILanguageServerFacade server;
        private readonly ICompilationProvider bicepCompilationContextProvider;
        private readonly IConfigurationManager bicepConfigurationManager;
        private readonly ConcurrentDictionary<DocumentUri, ParamsCompilationContext> activeContexts = new ConcurrentDictionary<DocumentUri, ParamsCompilationContext>();

        public BicepParamsCompilationManager(ILanguageServerFacade server, ICompilationProvider bicepCompilationContextProvider, IConfigurationManager bicepConfigurationManager)
        {
            this.server = server;
            this.bicepCompilationContextProvider = bicepCompilationContextProvider;
            this.bicepConfigurationManager = bicepConfigurationManager;
        }

        public void HandleFileChanges(IEnumerable<FileEvent> fileEvents) 
        {
            //TODO: complete later, not required for basic file interaction
        } 

        public void RefreshCompilation(DocumentUri uri, bool reloadBicepConfig = false)
        {
            //TODO: complete later, not required for basic file interaction
        } 

        public void UpsertCompilation(DocumentUri uri, int? version, string text, string? languageId = null)
        {

            var paramsFile = SourceFileFactory.CreateBicepParamFile(uri.ToUri(), text);

            var usingDeclarations = paramsFile.ProgramSyntax.Children.OfType<UsingDeclarationSyntax>();
            var bicepFilePath = PathHelper.TryGetUsingPath(usingDeclarations.SingleOrDefault(), out var getUsingPathFailureBuilder);

            if(bicepFilePath == null){
                throw new Exception("Path to Bicep file not provided");
            }

            if(!Uri.TryCreate(uri.ToUri(), bicepFilePath, out var bicepFileUri)){
                throw new Exception("Can not create Uri for linked Bicep file");
            }

            var bicepConfig = bicepConfigurationManager.GetConfiguration(bicepFileUri);

            var bicepCompilationContext = bicepCompilationContextProvider.Create(new Workspace(), bicepFileUri, new Dictionary<ISourceFile, ISemanticModel>().ToImmutableDictionary(), bicepConfig, new LinterAnalyzer(bicepConfig));

            var semanticModel = new ParamsSemanticModel(paramsFile, bicepCompilationContext.Compilation);

            var context = this.activeContexts.AddOrUpdate(uri, 
            (uri) => new ParamsCompilationContext(semanticModel, semanticModel.bicepParamFile.ProgramSyntax, semanticModel.bicepParamFile.LineStarts), 
            (uri, prevContext) => new ParamsCompilationContext(semanticModel, semanticModel.bicepParamFile.ProgramSyntax, semanticModel.bicepParamFile.LineStarts));

            this.PublishDocumentDiagnostics(uri, version, context.ParamsSemanticModel.GetDiagnostics(), context.LineStarts);  
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
