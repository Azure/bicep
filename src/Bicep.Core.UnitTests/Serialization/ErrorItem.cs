using System;
using Bicep.Core.Parser;
using Newtonsoft.Json;

namespace Bicep.Core.UnitTests.Serialization
{
    public class ErrorItem
    {
        [JsonConstructor]
        public ErrorItem(string message, TextSpan span, string spanText)
        {
            this.Message = message;
            this.Span = span;
            this.SpanText = spanText;
        }

        public ErrorItem(Error error, string contents)
        {
            this.Message = error.Message;
            this.Span = error.Span;
            this.SpanText = contents[new Range(error.Span.Position, error.Span.Position + error.Span.Length)];
        }

        public string Message { get; }

        [JsonConverter(typeof(TextSpanConverter))]
        public TextSpan Span { get; }

        // This isn't needed for testing but improves readability of spans in assertion files.
        public string SpanText { get; set; }
    }
}
