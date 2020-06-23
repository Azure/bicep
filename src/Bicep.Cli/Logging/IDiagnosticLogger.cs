using System.Collections.Immutable;
using Bicep.Core.Parser;

namespace Bicep.Cli.Logging
{
    public interface IDiagnosticLogger
    {
        void LogDiagnostic(string filePath, Error diagnostic, ImmutableArray<int> lineStarts);

        bool HasLoggedErrors { get; }
    }
}
