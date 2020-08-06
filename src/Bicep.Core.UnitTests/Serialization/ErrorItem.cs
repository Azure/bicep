using System;
using Bicep.Core.Errors;
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

        public ErrorItem(Error error, string contents)
        {
            this.Message = error.Message;
            this.ErrorCode = error.ErrorCode;
            this.Span = error.Span;
            this.SpanText = contents[new Range(error.Span.Position, error.Span.Position + error.Span.Length)];
        }

        public string Message { get; }

        public string ErrorCode { get; }

        [JsonConverter(typeof(TextSpanConverter))]
        public TextSpan Span { get; }

        // This isn't needed for testing but improves readability of spans in assertion files.
        public string SpanText { get; set; }
    }
}
