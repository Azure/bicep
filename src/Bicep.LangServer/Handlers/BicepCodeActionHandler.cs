// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Xml;
using Bicep.Core;
using Bicep.Core.Analyzers;
using Bicep.Core.CodeAction;
using Bicep.Core.CodeAction.Fixes;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Telemetry;
using Bicep.LanguageServer.Utils;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using static Bicep.LanguageServer.Completions.BicepCompletionContext;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer.Handlers
{
    // Provides code actions/fixes for a range in a Bicep document
    public class BicepCodeActionHandler : CodeActionHandlerBase
    {
        private const int MaxExpressionLengthInAction = 100;
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

            var requestStartOffset = PositionHelper.GetOffset(compilationContext.LineStarts, request.Range.Start); //asdfg refactor (and below)
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
                .SelectMany(fixable => fixable.Fixes.Select(fix => CreateCodeAction(request.TextDocument.Uri, compilationContext, fix)));

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
                    .OfType<AnalyzerDiagnostic>()
                    .Select(analyzerDiagnostic => CreateEditLinterRuleAction(documentUri, analyzerDiagnostic.Code, semanticModel.Configuration.ConfigFileUri?.LocalPath));
                commandOrCodeActions.AddRange(editLinterRuleActions);
            }

            var nodesInRange = SyntaxMatcher.FindNodesInRange(compilationContext.ProgramSyntax, requestStartOffset, requestEndOffset);
            var codeFixes = GetDecoratorCodeFixProviders(semanticModel)
                .SelectMany(provider => provider.GetFixes(semanticModel, nodesInRange))
                .Select(fix => CreateCodeAction(request.TextDocument.Uri, compilationContext, fix));
            commandOrCodeActions.AddRange(codeFixes);

            commandOrCodeActions.AddRange(
                GetExtractionRefactorings(request, compilationContext, compilation, semanticModel, nodesInRange)
                .Select(fix => CreateCodeAction(documentUri, compilationContext, fix)));

            return new(commandOrCodeActions);
        }

        // asdfg all params needed?
        private static IEnumerable<CodeFix> GetExtractionRefactorings(CodeActionParams request, CompilationContext compilationContext, Compilation compilation, SemanticModel semanticModel, List<SyntaxBase> nodesInRange)
        {
            if (SyntaxMatcher.FindLastNodeOfType<ExpressionSyntax, ExpressionSyntax>(nodesInRange) is not (ExpressionSyntax expressionSyntax, _))
            {
                yield break;
            }

            var defaultVarName = "newVariable";

            // Semi-intelligent default names for new variable
            if (semanticModel.Binder.GetParent(expressionSyntax) is ObjectPropertySyntax propertySyntax
                && propertySyntax.TryGetKeyText() is string propertyName)
            {
                // objectPropertyName: <expression> => use objectPropertyName as default name
                defaultVarName = propertyName;
            }
            else if (expressionSyntax is ObjectPropertySyntax propertySyntax2
                && propertySyntax2.TryGetKeyText() is string propertyName2)
            {
                //asdfg combine with last
                // objectPropertyName: <expression> => use objectPropertyName as default name
                defaultVarName = propertyName2;

                var propertyValueSyntax = propertySyntax2.Value as ExpressionSyntax;
                if (propertyValueSyntax != null)
                {
                    expressionSyntax = propertyValueSyntax;
                }
                else
                {
                    yield break;
                }
            }
            else if (expressionSyntax is PropertyAccessSyntax propertyAccessSyntax)
            {
                // object.topPropertyName.propertyName and similar => use topPropertyName.propertyName as default variable name
                // Only consider two levels
                string lastPartName = propertyAccessSyntax.PropertyName.IdentifierName;
                var parent = propertyAccessSyntax.BaseExpression;
                string? firstPartName = parent switch {
                    PropertyAccessSyntax propertyAccess => propertyAccess.PropertyName.IdentifierName,
                    VariableAccessSyntax variableAccess => variableAccess.Name.IdentifierName,
                    FunctionCallSyntax functionCall => functionCall.Name.IdentifierName,
                    _ => null
                };

                defaultVarName = firstPartName is { } ? firstPartName + lastPartName.UppercaseFirstLetter() : lastPartName;
            }

            var activeScopes = ActiveScopesVisitor.GetActiveScopes(compilation.GetEntrypointSemanticModel().Root, expressionSyntax.Span.Position);
            for (int i = 1; i < int.MaxValue; ++i)
            {
                var tryingName = $"{defaultVarName}{(i < 2 ? "" : i)}";
                if (!activeScopes.Any(s => s.GetDeclarationsByName(tryingName).Any()))
                {
                    defaultVarName = tryingName;
                    break;
                }
            }

            if (semanticModel.Binder.GetNearestAncestor<StatementSyntax>(expressionSyntax) is not StatementSyntax statementSyntax)
            {
                yield break;
            }

            var declarationSyntax = SyntaxFactory.CreateVariableDeclaration(defaultVarName, expressionSyntax);
            //var newline = semanticModel.Configuration.Formatting.Data.NewlineKind.ToEscapeSequence(); //asdfg exctract
            //var declarationText = SyntaxStringifier.Stringify(declarationSyntax) + "\n"; //asdfg \n okay?
            var declarationText = PrettyPrinterV2.PrintValid(declarationSyntax, PrettyPrinterV2Options.Default)
                + "\n"; //asdfg \n okay?
            //asdfg var declarationText = $"{declarationSyntax}\n"; 
            var statementLine = TextCoordinateConverter.GetPosition(compilationContext.LineStarts, statementSyntax.Span.Position).line;
            var declarationReplacementSpan = new TextSpan(
                TextCoordinateConverter.GetOffset(compilationContext.LineStarts, statementLine, 0),
                0);

            yield return new CodeFix(
                $"Create variable for {GetQuotedExpressionText(expressionSyntax)}",
                isPreferred: false,
                CodeFixKind.RefactorExtract,
                new CodeReplacement(expressionSyntax.Span, defaultVarName),
                new CodeReplacement(declarationReplacementSpan, declarationText));
        }

        private static string GetQuotedExpressionText(ExpressionSyntax expressionSyntax)
        {
            return "\""
                + SyntaxStringifier.Stringify(expressionSyntax, newlineReplacement: " ")
                    .TruncateWithEllipses(MaxExpressionLengthInAction)
                    .Trim()
                + "\"";
        }

        private IEnumerable<DecoratorCodeFixProvider> GetDecoratorCodeFixProviders(SemanticModel semanticModel)
        {
            var nsResolver = semanticModel.Binder.NamespaceResolver;
            var a = nsResolver.GetNamespaceNames().Select(nsResolver.TryGetNamespace).WhereNotNull().ToArray();
            var b = a.SelectMany(ns => ns.DecoratorResolver.GetKnownDecoratorFunctions().Select(kvp => (ns, kvp.Key, kvp.Value))).ToArray();
            var c = b.ToLookup(t => t.Key);
            var d = c.SelectMany(grouping => grouping.Count() > 1
                    ? grouping.SelectMany(tuple => tuple.Value.Overloads.Select(tuple.ns.DecoratorResolver.TryGetDecorator).WhereNotNull().Select(decorator => ($"{tuple.ns.Name}.{tuple.Key}", decorator)))
                    : grouping.SelectMany(tuple => tuple.Value.Overloads.Select(tuple.ns.DecoratorResolver.TryGetDecorator).WhereNotNull().Select(decorator => (tuple.Key, decorator))))
                .ToArray();
            var e = d.Select(t => new DecoratorCodeFixProvider(t.Item1, t.decorator)).ToArray();

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

        private static CommandOrCodeAction CreateEditLinterRuleAction(DocumentUri documentUri, string ruleName, string? bicepConfigFilePath)
        {
            return new CodeAction
            {
                Title = String.Format(LangServerResources.EditLinterRuleActionTitle, ruleName),
                Command = TelemetryHelper.CreateCommand
                (
                    title: "edit linter rule code action",
                    name: LangServerConstants.EditLinterRuleCommandName,
                    args: JArray.FromObject(new List<object> { documentUri, ruleName, bicepConfigFilePath ?? string.Empty /* (passing null not allowed) */ })
                )
            };
        }

        public override Task<CodeAction> Handle(CodeAction request, CancellationToken cancellationToken)
        {
            // we are currently precomputing our quickfixes, so there's no need to resolve them after they are chosen
            // this shouldn't be called because registration options disabled the resolve functionality
            return Task.FromResult(request);
        }

        private static CommandOrCodeAction CreateCodeAction(DocumentUri uri, CompilationContext context, CodeFix fix)
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
                }
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
