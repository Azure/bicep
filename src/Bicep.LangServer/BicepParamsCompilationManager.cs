// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.LanguageServer.CompilationManager;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Bicep.LanguageServer.Extensions;
using Bicep.Core.Diagnostics;

namespace Bicep.LanguageServer
{
    public class BicepParamsCompilationManager : IParamsCompilationManager
    {   
        private readonly ILanguageServerFacade server;

        private readonly ConcurrentDictionary<DocumentUri, ParamsCompilationContext> activeContexts = new ConcurrentDictionary<DocumentUri, ParamsCompilationContext>();

        public BicepParamsCompilationManager(ILanguageServerFacade server) => this.server = server;

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
            var parser = new ParamsParser(text);
            var programSyntax = parser.Program();
            var lineStarts = TextCoordinateConverter.GetLineStarts(text);

            var foo = this.activeContexts.AddOrUpdate(uri, 
            (uri) => new ParamsCompilationContext(programSyntax, lineStarts), 
            (uri, prevContext) => new ParamsCompilationContext(programSyntax, lineStarts));

            this.PublishDocumentDiagnostics(uri, version, foo.ProgramSyntax.GetParseDiagnostics(),lineStarts);  
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
