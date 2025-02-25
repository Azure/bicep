// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core;
using Bicep.Core.Analyzers;
using Bicep.Core.CodeAction;
using Bicep.Core.CodeAction.Fixes;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.Workspaces;
using Bicep.IO.Abstraction;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Model;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Refactor;
using Bicep.LanguageServer.Telemetry;
using Bicep.LanguageServer.Utils;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer.Handlers
{
    // Provides code actions/fixes for a range in a Bicep document
    public class BicepCodeActionHandler : CodeActionHandlerBase
    {
        private readonly IClientCapabilitiesProvider clientCapabilitiesProvider;
        private readonly ICompilationManager compilationManager;
        private readonly DocumentSelectorFactory documentSelectorFactory;

        public BicepCodeActionHandler(ICompilationManager compilationManager, IClientCapabilitiesProvider clientCapabilitiesProvider, DocumentSelectorFactory documentSelectorFactory)
        {
            this.clientCapabilitiesProvider = clientCapabilitiesProvider;
            this.compilationManager = compilationManager;
            this.documentSelectorFactory = documentSelectorFactory;
        }

        public override async Task<CommandOrCodeActionContainer?> Handle(CodeActionParams request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            cancellationToken.ThrowIfCancellationRequested();

            var documentUri = request.TextDocument.Uri;
            var compilationContext = this.compilationManager.GetCompilation(documentUri);

            if (compilationContext == null)
            {
                return null;
            }

            var requestStartOffset = PositionHelper.GetOffset(compilationContext.LineStarts, request.Range.Start);
            var requestEndOffset = request.Range.Start != request.Range.End
                ? PositionHelper.GetOffset(compilationContext.LineStarts, request.Range.End)
                : requestStartOffset;

            var compilation = compilationContext.Compilation;
            var semanticModel = compilation.GetEntrypointSemanticModel();
            var diagnostics = semanticModel.GetAllDiagnostics();

            var quickFixes = diagnostics
                .Where(fixable =>
                    fixable.Span.ContainsInclusive(requestStartOffset) ||
                    fixable.Span.ContainsInclusive(requestEndOffset) ||
                    (requestStartOffset <= fixable.Span.Position && fixable.GetEndPosition() <= requestEndOffset))
                .OfType<IFixable>()
                .SelectMany(fixable => fixable.Fixes.Select(fix => CreateCodeActionFromFix(request.TextDocument.Uri, compilationContext, fix)));

            List<CommandOrCodeAction> commandOrCodeActions = new();

            commandOrCodeActions.AddRange(quickFixes);

            var coreCompilerErrors = diagnostics
                .Where(diagnostic => !diagnostic.CanBeSuppressed());
            var diagnosticsThatCanBeSuppressed = diagnostics
                .Where(diagnostic =>
                      diagnostic.Span.ContainsInclusive(requestStartOffset) ||
                      diagnostic.Span.ContainsInclusive(requestEndOffset) ||
                      (requestStartOffset <= diagnostic.Span.Position && diagnostic.GetEndPosition() <= requestEndOffset))
                .Except(coreCompilerErrors);

            HashSet<string> diagnosticCodesToSuppressInline = new();
            foreach (IDiagnostic diagnostic in diagnosticsThatCanBeSuppressed)
            {
                if (!diagnosticCodesToSuppressInline.Contains(diagnostic.Code))
                {
                    diagnosticCodesToSuppressInline.Add(diagnostic.Code);

                    var commandOrCodeAction = DisableDiagnostic(documentUri, diagnostic.Code, semanticModel.SourceFile, diagnostic.Span, compilationContext.LineStarts);
                    if (commandOrCodeAction is not null)
                    {
                        commandOrCodeActions.Add(commandOrCodeAction);
                    }
                }
            }

            if (clientCapabilitiesProvider.DoesClientSupportShowDocumentRequest())
            {
                // Add "Edit <rule> in bicepconfig.json" for all linter failures
                var editLinterRuleActions = diagnostics
                    .Where(analyzerDiagnostic =>
                        analyzerDiagnostic.Span.ContainsInclusive(requestStartOffset) ||
                        analyzerDiagnostic.Span.ContainsInclusive(requestEndOffset) ||
                        (requestStartOffset <= analyzerDiagnostic.Span.Position && analyzerDiagnostic.GetEndPosition() <= requestEndOffset))
                    .Where(x => x.Source == DiagnosticSource.CoreLinter)
                    .Select(analyzerDiagnostic => CreateEditLinterRuleAction(documentUri, analyzerDiagnostic.Code, semanticModel.Configuration.ConfigFileUri));
                commandOrCodeActions.AddRange(editLinterRuleActions);
            }

            var nodesInRange = SyntaxMatcher.FindNodesSpanningRange(compilationContext.ProgramSyntax, requestStartOffset, requestEndOffset);
            var codeFixes = GetCodeFixes(semanticModel, nodesInRange);
            commandOrCodeActions.AddRange(codeFixes.Select(fix => CreateCodeActionFromFix(documentUri, compilationContext, fix)));

            return new(commandOrCodeActions);
        }

        private static IEnumerable<CodeFix> GetCodeFixes(SemanticModel model, IReadOnlyList<SyntaxBase> matchingNodes)
        {
            ICodeFixProvider[] providers = [
                .. GetDecoratorCodeFixProviders(model),
                new ExpressionAndTypeExtractor(model),
                new MultilineStringCodeFixProvider(),
            ];

            return providers
                .SelectMany(provider => provider.GetFixes(model, matchingNodes));
        }

        private static IEnumerable<DecoratorCodeFixProvider> GetDecoratorCodeFixProviders(SemanticModel semanticModel)
        {
            var nsResolver = semanticModel.Binder.NamespaceResolver;
            return nsResolver.GetNamespaceNames().Select(nsResolver.TryGetNamespace).WhereNotNull()
                .SelectMany(ns => ns.DecoratorResolver.GetKnownDecoratorFunctions().Select(kvp => (ns, kvp.Key, kvp.Value)))
                .ToLookup(t => t.Key)
                .SelectMany(grouping => grouping.Count() > 1
                    ? grouping.SelectMany(tuple => tuple.Value.Overloads.Select(tuple.ns.DecoratorResolver.TryGetDecorator).WhereNotNull().Select(decorator => ($"{tuple.ns.Name}.{tuple.Key}", decorator)))
                    : grouping.SelectMany(tuple => tuple.Value.Overloads.Select(tuple.ns.DecoratorResolver.TryGetDecorator).WhereNotNull().Select(decorator => (tuple.Key, decorator))))
                .Select(t => new DecoratorCodeFixProvider(t.Item1, t.decorator));
        }

        private static CommandOrCodeAction? DisableDiagnostic(DocumentUri documentUri,
            DiagnosticCode diagnosticCode,
            BicepSourceFile bicepFile,
            TextSpan span,
            ImmutableArray<int> lineStarts)
        {
            if (diagnosticCode.String is null)
            {
                return null;
            }

            var disabledDiagnosticsCache = bicepFile.DisabledDiagnosticsCache;
            (int diagnosticLine, _) = TextCoordinateConverter.GetPosition(bicepFile.LineStarts, span.Position);

            TextEdit? textEdit;
            int previousLine = diagnosticLine - 1;
            if (disabledDiagnosticsCache.TryGetDisabledNextLineDirective(previousLine) is { } disableNextLineDirectiveEndPositionAndCodes)
            {
                textEdit = new TextEdit
                {
                    Range = new Range(previousLine, disableNextLineDirectiveEndPositionAndCodes.endPosition, previousLine, disableNextLineDirectiveEndPositionAndCodes.endPosition),
                    NewText = ' ' + diagnosticCode.String
                };
            }
            else
            {
                var range = span.ToRange(lineStarts);
                textEdit = new TextEdit
                {
                    Range = new Range(range.Start.Line, 0, range.Start.Line, 0),
                    NewText = "#" + LanguageConstants.DisableNextLineDiagnosticsKeyword + ' ' + diagnosticCode.String + '\n'
                };
            }

            BicepTelemetryEvent telemetryEvent = BicepTelemetryEvent.CreateDisableNextLineDiagnostics(diagnosticCode.String);
            var telemetryCommand = TelemetryHelper.CreateCommand(
                title: "disable next line diagnostics code action",
                name: TelemetryConstants.CommandName,
                args: JArray.FromObject(new List<object> { telemetryEvent })
            );

            return new CodeAction
            {
                Title = string.Format(LangServerResources.DisableDiagnosticForThisLine, diagnosticCode.String),
                Edit = new WorkspaceEdit
                {
                    Changes = new Dictionary<DocumentUri, IEnumerable<TextEdit>>
                    {
                        [documentUri] = new List<TextEdit> { textEdit }
                    }
                },
                Command = telemetryCommand
            };
        }

        private static CommandOrCodeAction CreateEditLinterRuleAction(DocumentUri documentUri, string ruleName, IOUri? configFileIdentifier)
        {
            return new CodeAction
            {
                Title = String.Format(LangServerResources.EditLinterRuleActionTitle, ruleName),
                Command = TelemetryHelper.CreateCommand
                (
                    title: "edit linter rule code action",
                    name: LangServerConstants.EditLinterRuleCommandName,
                    args: JArray.FromObject(new List<object> { documentUri, ruleName, configFileIdentifier?.ToString() ?? string.Empty /* (passing null not allowed) */ })
                )
            };
        }

        public override Task<CodeAction> Handle(CodeAction request, CancellationToken cancellationToken)
        {
            // we are currently precomputing our quickfixes, so there's no need to resolve them after they are chosen
            // this shouldn't be called because registration options disabled the resolve functionality
            return Task.FromResult(request);
        }

        private static CommandOrCodeAction CreateCodeActionFromFix(DocumentUri uri, CompilationContext context, CodeFix fix)
        {
            var codeActionKind = fix.Kind switch
            {
                CodeFixKind.QuickFix => CodeActionKind.QuickFix,
                CodeFixKind.Refactor => CodeActionKind.Refactor,
                CodeFixKind.RefactorExtract => CodeActionKind.RefactorExtract,
                _ => CodeActionKind.Empty,
            };

            return new CodeAction
            {
                Kind = codeActionKind,
                Title = fix.Title,
                IsPreferred = fix.IsPreferred,
                Edit = new WorkspaceEdit
                {
                    Changes = new Dictionary<DocumentUri, IEnumerable<TextEdit>>
                    {
                        [uri] = fix.Replacements.Select(replacement => new TextEdit
                        {
                            Range = replacement.ToRange(context.LineStarts),
                            NewText = replacement.Text
                        })
                    }
                },
                Command = fix is CodeFixWithCommand withCommand ? withCommand.Command : null,
            };
        }

        protected override CodeActionRegistrationOptions CreateRegistrationOptions(CodeActionCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = documentSelectorFactory.CreateForBicepAndParams(),
            CodeActionKinds = new Container<CodeActionKind>(CodeActionKind.QuickFix),
            ResolveProvider = false
        };
    }
}
