// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Highlighting;
using Bicep.Core.Semantics;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using LspTokenType = OmniSharp.Extensions.LanguageServer.Protocol.Models.SemanticTokenType;
using BicepTokenType = Bicep.Core.Highlighting.SemanticTokenType;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepSemanticTokensHandler(ICompilationManager compilationManager, DocumentSelectorFactory documentSelectorFactory) : SemanticTokensHandlerBase
    {
        // TODO: Not sure if this needs to be shared.
        private readonly SemanticTokensLegend legend = new();

        protected override Task<SemanticTokensDocument> GetSemanticTokensDocument(ITextDocumentIdentifierParams @params, CancellationToken cancellationToken)
        {
            return Task.FromResult(new SemanticTokensDocument(this.legend));
        }

        protected override Task Tokenize(SemanticTokensBuilder builder, ITextDocumentIdentifierParams identifier, CancellationToken cancellationToken)
        {
            /*
             * do not check for file extension here because that will prevent untitled files from getting syntax highlighting
             */

            var compilationContext = compilationManager.GetCompilation(identifier.TextDocument.Uri);
            if (compilationContext is not null)
            {
                BuildSemanticTokens(builder, compilationContext.Compilation.GetEntrypointSemanticModel());
            }

            return Task.CompletedTask;
        }

        public static void BuildSemanticTokens(SemanticTokensBuilder builder, SemanticModel model)
        {
            var result = SemanticTokenVisitor.Build(model);

            // the builder is fussy about ordering. tokens are visited out of order, we need to call build after visiting everything
            foreach (var (positionable, tokenType) in result.OrderBy(t => t.Positionable.Span.Position))
            {
                var tokenRanges = positionable.ToRangeSpanningLines(model.SourceFile.LineStarts);
                foreach (var tokenRange in tokenRanges)
                {
                    builder.Push(tokenRange.Start.Line, tokenRange.Start.Character, tokenRange.End.Character - tokenRange.Start.Character, MapTokenType(tokenType));
                }
            }
        }

        public static LspTokenType? MapTokenType(BicepTokenType tokenType) => tokenType switch
        {
            BicepTokenType.Comment => LspTokenType.Comment,
            BicepTokenType.EnumMember => LspTokenType.EnumMember,
            BicepTokenType.Event => LspTokenType.Event,
            BicepTokenType.Modifier => LspTokenType.Modifier,
            BicepTokenType.Label => LspTokenType.Label,
            BicepTokenType.Parameter => LspTokenType.Parameter,
            BicepTokenType.Variable => LspTokenType.Variable,
            BicepTokenType.Property => LspTokenType.Property,
            BicepTokenType.Function => LspTokenType.Function,
            BicepTokenType.TypeParameter => LspTokenType.TypeParameter,
            BicepTokenType.Macro => LspTokenType.Macro,
            BicepTokenType.Interface => LspTokenType.Interface,
            BicepTokenType.Enum => LspTokenType.Enum,
            BicepTokenType.String => LspTokenType.String,
            BicepTokenType.Number => LspTokenType.Number,
            BicepTokenType.Regexp => LspTokenType.Regexp,
            BicepTokenType.Operator => LspTokenType.Operator,
            BicepTokenType.Keyword => LspTokenType.Keyword,
            BicepTokenType.Type => LspTokenType.Type,
            BicepTokenType.Struct => LspTokenType.Struct,
            BicepTokenType.Class => LspTokenType.Class,
            BicepTokenType.Namespace => LspTokenType.Namespace,
            BicepTokenType.Decorator => LspTokenType.Decorator,
            _ => throw new NotImplementedException($"No mapping for token type {tokenType}"),
        };

        protected override SemanticTokensRegistrationOptions CreateRegistrationOptions(SemanticTokensCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            // the semantic tokens handler requests don't get routed like other handlers
            // it seems we can only have one and it must be shared between all the language IDs we support
            DocumentSelector = documentSelectorFactory.CreateForBicepAndParams(),
            Legend = this.legend,
            Full = new SemanticTokensCapabilityRequestFull
            {
                Delta = true
            },
            Range = true
        };
    }
}
