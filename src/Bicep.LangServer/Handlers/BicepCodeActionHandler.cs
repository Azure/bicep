// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Analyzers;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Telemetry;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepCodeActionHandler : CodeActionHandlerBase
    {
        private readonly ICompilationManager compilationManager;

        public BicepCodeActionHandler(ICompilationManager compilationManager)
        {
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
                .SelectMany(fixable => fixable.Fixes.Select(fix => CreateQuickFix(request.TextDocument.Uri, compilationContext, fix)));

            List<CommandOrCodeAction> commandOrCodeActions = new();

            commandOrCodeActions.AddRange(quickFixes);

            var analyzerDiagnostics = diagnostics
                .Where(analyzerDiagnostic =>
                    analyzerDiagnostic.Span.ContainsInclusive(requestStartOffset) ||
                    analyzerDiagnostic.Span.ContainsInclusive(requestEndOffset) ||
                    (requestStartOffset <= analyzerDiagnostic.Span.Position && analyzerDiagnostic.GetEndPosition() <= requestEndOffset))
                .OfType<AnalyzerDiagnostic>()
                .Select(analyzerDiagnostic => DisableLinterRule(documentUri, analyzerDiagnostic.Code, compilation.Configuration.ConfigurationPath));

            commandOrCodeActions.AddRange(analyzerDiagnostics);

            var diagnosticsOfTypeWarningAndInfo = diagnostics
                .Where(diagnostic =>
                    (diagnostic.Level == DiagnosticLevel.Warning || diagnostic.Level == DiagnosticLevel.Info) &&
                    (diagnostic.Span.ContainsInclusive(requestStartOffset) ||
                    diagnostic.Span.ContainsInclusive(requestEndOffset) ||
                    (requestStartOffset <= diagnostic.Span.Position && diagnostic.GetEndPosition() <= requestEndOffset)))
                .OfType<IDiagnostic>();

            HashSet<string> diagnosticCodesToSuppressInline = new();

            foreach (IDiagnostic diagnostic in diagnosticsOfTypeWarningAndInfo)
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

            var syntaxTrivia = DisableDiagnosticsHelper.GetDisableNextLineDiagnosticStatementFromPreviousLine(bicepFile.ProgramSyntax,
                                                                                                              bicepFile.LineStarts,
                                                                                                              span.Position,
                                                                                                              out int previousLine);
            TextEdit? disableDiagnosticStart;

            if (syntaxTrivia is null)
            {
                var declarationRange = span.ToRange(lineStarts);
                disableDiagnosticStart = new TextEdit
                {
                    Range = new Range(declarationRange.Start.Line, 0, declarationRange.Start.Line, 0),
                    NewText = "#" + LanguageConstants.DisableNextLineDiagnosticsKeyword + ' ' + diagnosticCode.String + '\n'
                };
            }
            else
            {
                int position = syntaxTrivia.GetEndPosition();
                disableDiagnosticStart = new TextEdit
                {
                    Range = new Range(previousLine, position, previousLine, position),
                    NewText = ' ' + diagnosticCode.String
                };
            }

            BicepTelemetryEvent telemetryEvent = BicepTelemetryEvent.CreateDisableNextLineDiagnostics(diagnosticCode.String);
            var telemetryCommand = Command.Create(TelemetryConstants.CommandName, telemetryEvent);

            return new CodeAction
            {
                Title = string.Format("Disable {0}", diagnosticCode.String),
                Edit = new WorkspaceEdit
                {
                    Changes = new Dictionary<DocumentUri, IEnumerable<TextEdit>>
                    {
                        [documentUri] = new List<TextEdit> { disableDiagnosticStart }
                    }
                },
                Command = telemetryCommand
            };
        }

        private static CommandOrCodeAction DisableLinterRule(DocumentUri documentUri, string ruleName, string? bicepConfigFilePath)
        {
            var command = Command.Create(LanguageConstants.DisableLinterRuleCommandName, documentUri, ruleName, bicepConfigFilePath ?? string.Empty);

            return new CodeAction
            {
                Title = LangServerResources.DisableLinterRule,
                Command = command
            };
        }

        public override Task<CodeAction> Handle(CodeAction request, CancellationToken cancellationToken)
        {
            // we are currently precomputing our quickfixes, so there's no need to resolve them after they are chosen
            // this shouldn't be called because registration options disabled the resolve functionality
            return Task.FromResult(request);
        }

        private static CommandOrCodeAction CreateQuickFix(DocumentUri uri, CompilationContext context, CodeFix fix)
        {
            return new CodeAction
            {
                Kind = CodeActionKind.QuickFix,
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
