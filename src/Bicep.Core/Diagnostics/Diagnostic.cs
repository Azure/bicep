// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics;
using Bicep.Core.Parsing;

namespace Bicep.Core.Diagnostics
{
    // roughly equivalent to the 'SyntaxDiagnosticInfo' class in Roslyn
    [DebuggerDisplay("Level = {" + nameof(Level) + "}, Code = {" + nameof(Code) + "}, Message = {" + nameof(Message) + "}")]
    public class Diagnostic(
        TextSpan span,
        DiagnosticLevel level,
        string code,
        string message,
        Uri? documentationUri = null,
        DiagnosticStyling styling = DiagnosticStyling.Default,
        string? source = null) : IDiagnostic
    {
        public string Source { get; protected set; } = source ?? LanguageConstants.LanguageId;

        public TextSpan Span { get; } = span;

        public DiagnosticLevel Level { get; } = level;

        public DiagnosticStyling Styling { get; } = styling;

        public string Code { get; } = code;

        public string Message { get; } = message;

        public Uri? Uri { get; } = documentationUri;

    }
}
