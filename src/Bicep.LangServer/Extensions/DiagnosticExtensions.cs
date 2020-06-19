using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Parser;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Extensions
{
    public static class DiagnosticExtensions
    {
        public static IEnumerable<Diagnostic> ToDiagnostics(this IEnumerable<Error> source, string text)
        {
            IReadOnlyList<int> lineStarts = PositionHelper.GetLineStarts(text);

            return source.Select(error => new Diagnostic
            {
                Severity = DiagnosticSeverity.Error,
                Message = error.Message,
                Source = LanguageServerConstants.LanguageId,
                Range = new Range
                {
                    Start = PositionHelper.GetPosition(lineStarts, error.Span.Position),
                    End = PositionHelper.GetPosition(lineStarts, error.Span.Position + error.Span.Length)
                }
            });
        }
    }
}
