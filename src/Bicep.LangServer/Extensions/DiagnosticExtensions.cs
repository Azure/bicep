using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Diagnostic = Bicep.Core.Diagnostics.Diagnostic;

namespace Bicep.LanguageServer.Extensions
{
    public static class DiagnosticExtensions
    {
        public static IEnumerable<OmniSharp.Extensions.LanguageServer.Protocol.Models.Diagnostic> ToDiagnostics(this IEnumerable<Diagnostic> source, ImmutableArray<int> lineStarts) =>
            source.Select(diagnostic => new OmniSharp.Extensions.LanguageServer.Protocol.Models.Diagnostic
            {
                Severity = DiagnosticSeverity.Error,
                Code = diagnostic.Code,
                Message = diagnostic.Message,
                Source = LanguageServerConstants.LanguageId,
                Range = diagnostic.ToRange(lineStarts)
            });
    }
}
