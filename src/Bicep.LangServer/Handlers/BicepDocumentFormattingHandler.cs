// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.PrettyPrint;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Bicep.Core.PrettyPrint.Options;
using Microsoft.Extensions.Logging;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.Extensions;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepDocumentFormattingHandler : DocumentFormattingHandler
    {
        private readonly ILogger<BicepDocumentSymbolHandler> logger;
        private readonly ICompilationManager compilationManager;

        public BicepDocumentFormattingHandler(ILogger<BicepDocumentSymbolHandler> logger, ICompilationManager compilationManager)
            : base(CreateRegistrationOptions())
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
        }

        public override Task<TextEditContainer?> Handle(DocumentFormattingParams request, CancellationToken cancellationToken)
        {
            CompilationContext? context = this.compilationManager.GetCompilation(request.TextDocument.Uri);

            if (context == null)
            {
                // we have not yet compiled this document, which shouldn't really happen
                this.logger.LogError("Document formatting request arrived before file {Uri} could be compiled.", request.TextDocument.Uri);

                return Task.FromResult<TextEditContainer?>(null);
            }

            long indentSize = request.Options.TabSize;
            IndentKindOption indentKindOption = request.Options.InsertSpaces ? IndentKindOption.Space : IndentKindOption.Tab;
            bool insertFinalNewline = request.Options.InsertFinalNewline;

            ProgramSyntax programSyntax = context.ProgramSyntax;
            PrettyPrintOptions options = new PrettyPrintOptions(NewlineOption.Auto, indentKindOption, indentSize, insertFinalNewline);

            string? output = PrettyPrinter.PrintProgram(programSyntax, options);

            if (output == null)
            {
                return Task.FromResult<TextEditContainer?>(null);
            }

            return Task.FromResult<TextEditContainer?>(new TextEditContainer(new TextEdit
            {
                Range = programSyntax.FullSpan.ToRange(context.LineStarts),
                NewText = output
            }));
        }

        private static DocumentFormattingRegistrationOptions CreateRegistrationOptions() =>
            new DocumentFormattingRegistrationOptions
            {
                DocumentSelector = DocumentSelectorFactory.Create()
            };
    }
}
