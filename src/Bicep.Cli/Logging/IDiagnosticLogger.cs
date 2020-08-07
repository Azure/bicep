using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parser;

namespace Bicep.Cli.Logging
{
    public interface IDiagnosticLogger
    {
        void LogDiagnostic(string filePath, Diagnostic diagnostic, ImmutableArray<int> lineStarts);

        bool HasLoggedErrors { get; }
    }
}
