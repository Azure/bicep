using System;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parser;
using Newtonsoft.Json;

namespace Bicep.Core.UnitTests.Serialization
{
    public class ErrorItem
    {
        [JsonConstructor]
        public ErrorItem(string message, string ErrorCode, TextSpan span, string spanText)
        {
            this.Message = message;
            this.ErrorCode = ErrorCode;
            this.Span = span;
            this.SpanText = spanText;
        }

        public ErrorItem(ErrorDiagnostic diagnostic, string contents)
        {
            this.Message = diagnostic.Message;
            this.ErrorCode = diagnostic.Code;
            this.Span = diagnostic.Span;
            this.SpanText = contents[new Range(diagnostic.Span.Position, diagnostic.Span.Position + diagnostic.Span.Length)];
        }

        public string Message { get; }

        public string ErrorCode { get; }

        [JsonConverter(typeof(TextSpanConverter))]
        public TextSpan Span { get; }

        // This isn't needed for testing but improves readability of spans in assertion files.
        public string SpanText { get; set; }
    }
}
