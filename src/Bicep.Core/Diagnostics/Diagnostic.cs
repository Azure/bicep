// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Diagnostics;
using Bicep.Core.Parsing;

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
            DiagnosticLabel? label = null,
            string? source = null)
        {
            Span = span;
            Level = level;
            Code = code;
            Message = message;
            Label = label;
            Uri = documentationUri;
            Source = source ?? LanguageConstants.LanguageId;
        }

        public string Source { get; protected set; }
     
        public TextSpan Span { get; }

        public DiagnosticLevel Level { get; }

        public DiagnosticLabel? Label { get; }

        public string Code { get; }

        public string Message { get; }

        public Uri? Uri { get; }

    }
}
