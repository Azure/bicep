// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;
using System;
using System.Diagnostics;

namespace Bicep.Core.Diagnostics
{
    // roughly equivalent to the 'SyntaxDiagnosticInfo' class in Roslyn
    [DebuggerDisplay("Level = {" + nameof(Level) + "}, Code = {" + nameof(Code) + "}, Message = {" + nameof(Message) + "}")]
    public class Diagnostic : IDiagnostic
    {
        public Diagnostic(
            TextSpan span,
            DiagnosticLevel level,
            string code,
            string message,
            Uri? documentationUri = null,
            DiagnosticStyling styling = DiagnosticStyling.Default,
            string? source = null)
        {
            Span = span;
            Level = level;
            Code = code;
            Message = message;
            Styling = styling;
            Uri = documentationUri;
            Source = source ?? LanguageConstants.LanguageId;
        }

        public string Source { get; protected set; }

        public TextSpan Span { get; }

        public DiagnosticLevel Level { get; }

        public DiagnosticStyling Styling { get; }

        public string Code { get; }

        public string Message { get; }

        public Uri? Uri { get; }

    }
}
