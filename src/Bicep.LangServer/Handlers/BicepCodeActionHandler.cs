// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Analyzers;
using Bicep.Core.CodeAction;
using Bicep.Core.CodeAction.Fixes;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
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
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer.Handlers
{
    // Provides code actions/fixes for a range in a Bicep document
    public class BicepCodeActionHandler : CodeActionHandlerBase
    {
        private readonly IClientCapabilitiesProvider clientCapabilitiesProvider;
        private readonly ICompilationManager compilationManager;

        private static readonly ImmutableArray<ICodeFixProvider> codeFixProviders = new ICodeFixProvider[]
        {
            new ParameterCodeFixProvider("secure", new []{"string", "object"}, Array.Empty<SyntaxBase>()),
            new ParameterCodeFixProvider("description", new []{"string", "object", "array", "bool", "int"}, new []{SyntaxFactory.CreateStringLiteral(String.Empty)}),
            new ParameterCodeFixProvider("allowed", new []{"string", "object", "array", "bool", "int"}, new []{SyntaxFactory.CreateArray(Array.Empty<SyntaxBase>()) }),
            new ParameterCodeFixProvider("minLength", new []{"string", "array"}, Array.Empty<SyntaxBase>()),
            new ParameterCodeFixProvider("maxLength", new []{"string", "array"}, Array.Empty<SyntaxBase>()),
            new ParameterCodeFixProvider("minValue", new []{"int"}, Array.Empty<SyntaxBase>()),
            new ParameterCodeFixProvider("maxValue", new []{"int"}, Array.Empty<SyntaxBase>()),
            new MultilineObjectsAndArraysCodeFixProvider(),
        }.ToImmutableArray<ICodeFixProvider>();

        public BicepCodeActionHandler(ICompilationManager compilationManager, IClientCapabilitiesProvider clientCapabilitiesProvider)
        {
            this.clientCapabilitiesProvider = clientCapabilitiesProvider;
            this.compilationManager = compilationManager;
        }

        public override Task<CommandOrCodeActionContainer> Handle(CodeActionParams request, CancellationToken cancellationToken)
        {
            var documentUri = request.TextDocument.Uri;
            var compilationContext = this.compilationManager.GetCompilation(documentUri);

            if (compilationContext == null)
            {
                return Task.FromResult(new CommandOrCodeActionContainer());
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
                .SelectMany(fixable => fixable.Fixes.Select(fix => CreateCodeFix(request.TextDocument.Uri, compilationContext, fix)));

            List<CommandOrCodeAction> commandOrCodeActions = new();

            commandOrCodeActions.AddRange(quickFixes);

            var coreCompilerErrors = diagnostics
                .Where(diagnostic => !diagnostic.CanBeSuppressed());
            var diagnosticsThatCanBeSuppressed = diagnostics
                .Where(diagnostic =>
                      (diagnostic.Span.ContainsInclusive(requestStartOffset) ||
                      diagnostic.Span.ContainsInclusive(requestEndOffset) ||
                      (requestStartOffset <= diagnostic.Span.Position && diagnostic.GetEndPosition() <= requestEndOffset)))
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
                // Add "Edit <rule> in bicep.config" for all linter failures
                var editLinterRuleActions = diagnostics
                    .Where(analyzerDiagnostic =>
                        analyzerDiagnostic.Span.ContainsInclusive(requestStartOffset) ||
                        analyzerDiagnostic.Span.ContainsInclusive(requestEndOffset) ||
                        (requestStartOffset <= analyzerDiagnostic.Span.Position && analyzerDiagnostic.GetEndPosition() <= requestEndOffset))
                    .OfType<AnalyzerDiagnostic>()
                    .Select(analyzerDiagnostic => CreateEditLinterRuleAction(documentUri, analyzerDiagnostic.Code, semanticModel.Configuration.ConfigurationPath));
                commandOrCodeActions.AddRange(editLinterRuleActions);
            }

            var matchingNodes = SyntaxMatcher.FindNodesInRange(compilationContext.ProgramSyntax, requestStartOffset, requestEndOffset);
            var codeFixes = codeFixProviders
                .SelectMany(provider => provider.GetFixes(semanticModel, matchingNodes))
                .Select(fix => CreateCodeFix(request.TextDocument.Uri, compilationContext, fix));
            commandOrCodeActions.AddRange(codeFixes);

            return Task.FromResult(new CommandOrCodeActionContainer(commandOrCodeActions));
        }

        private static CommandOrCodeAction? DisableDiagnostic(DocumentUri documentUri,
            DiagnosticCode diagnosticCode,
            BicepFile bicepFile,
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

        private static CommandOrCodeAction CreateCodeFix(DocumentUri uri, CompilationContext context, CodeFix fix)
        {
            var codeActionKind = fix.Kind switch
            {
                CodeFixKind.QuickFix => CodeActionKind.QuickFix,
                CodeFixKind.Refactor => CodeActionKind.Refactor,
                _ => CodeActionKind.Empty,
            };

            return new CodeAction
            {
                Kind = codeActionKind,
                Title = fix.Description,
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
            DocumentSelector = DocumentSelectorFactory.Create(),
            CodeActionKinds = new Container<CodeActionKind>(CodeActionKind.QuickFix),
            ResolveProvider = false
        };
    }
}
