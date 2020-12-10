// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Newtonsoft.Json;

namespace Bicep.Core.UnitTests.Serialization
{
    public class DiagnosticItem
    {
        [JsonConstructor]
        public DiagnosticItem(string message, string ErrorCode, string level, TextSpan span, string spanText)
        {
            this.Message = message;
            this.ErrorCode = ErrorCode;
            this.Level = level;
            this.Span = span;
            this.SpanText = spanText;
        }

        public DiagnosticItem(Diagnostic diagnostic, string contents)
        {
            this.Message = diagnostic.Message;
            this.ErrorCode = diagnostic.Code;
            this.Level = diagnostic.Level.ToString();
            this.Span = diagnostic.Span;
            this.SpanText = contents[new Range(diagnostic.Span.Position, diagnostic.GetEndPosition())];
        }

        public string Message { get; }

        public string ErrorCode { get; }

        public string Level { get; }

        [JsonConverter(typeof(TextSpanConverter))]
        public TextSpan Span { get; }

        // This isn't needed for testing but improves readability of spans in assertion files.
        public string SpanText { get; set; }
    }
}

