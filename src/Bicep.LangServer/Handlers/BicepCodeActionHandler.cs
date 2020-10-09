// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Linter;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Extensions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepCodeActionHandler : CodeActionHandler
    {
        private readonly ICompilationManager compilationManager;

        public BicepCodeActionHandler(ICompilationManager compilationManager)
            : base(CreateCodeActionRegistrationOptions())
        {
            this.compilationManager = compilationManager;
        }
        
        public override Task<CommandOrCodeActionContainer> Handle(CodeActionParams request, CancellationToken cancellationToken)
        {
            var compilationContext = this.compilationManager.GetCompilation(request.TextDocument.Uri);

            if (compilationContext == null)
            {
                return Task.FromResult(new CommandOrCodeActionContainer());
            }

            var quickFixes = compilationContext.Compilation.GetSemanticModel().GetAllDiagnostics()
                .Where(diagnostic => diagnostic.Span.ToRange(compilationContext.LineStarts).Equals(request.Range))
                .OfType<IFixable>()
                .SelectMany(fixable => fixable.Fixes.Select(fix => GetQuickFix(request.TextDocument.Uri, compilationContext, fix)));

            return Task.FromResult(new CommandOrCodeActionContainer(quickFixes));
        }

        private static CommandOrCodeAction GetQuickFix(DocumentUri uri, CompilationContext context, Fix fix)
        {
            return new CodeAction
            {
                Kind = CodeActionKind.QuickFix,
                Title = fix.Description,
                Edit = new WorkspaceEdit
                {
                    Changes = new Dictionary<DocumentUri, IEnumerable<TextEdit>>
                    {
                        [uri] = fix.Edits.Select(edit => new TextEdit
                        {
                            Range = edit.ToRange(context.LineStarts),
                            NewText = edit.Text
                        })
                    }
                }
            };
        }

        private static CodeActionRegistrationOptions CreateCodeActionRegistrationOptions() => new CodeActionRegistrationOptions
        {
            DocumentSelector = new DocumentSelector(),
            CodeActionKinds = new Container<CodeActionKind>(CodeActionKind.QuickFix)
        };
    }
}
