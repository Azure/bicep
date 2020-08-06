using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Errors;
using Bicep.Core.Parser;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Extensions
{
    public static class DiagnosticExtensions
    {
        public static IEnumerable<Diagnostic> ToDiagnostics(this IEnumerable<Error> source, ImmutableArray<int> lineStarts) =>
            source.Select(error => new Diagnostic
            {
                Severity = DiagnosticSeverity.Error,
                Code = error.ErrorCode,
                Message = error.Message,
                Source = LanguageServerConstants.LanguageId,
                Range = error.ToRange(lineStarts)
            });
    }
}
