using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parser;
using Bicep.Core.Text;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Logging
{
    /// <summary>
    /// Wraps a dotnet logger and keeps track of error history.
    /// </summary>
    public class BicepDiagnosticLogger : IDiagnosticLogger
    {
        private readonly ILogger logger;

        public BicepDiagnosticLogger(ILogger logger)
        {
            this.logger = logger;
            this.HasLoggedErrors = false;
        }

        public void LogDiagnostic(string filePath, Diagnostic diagnostic, ImmutableArray<int> lineStarts)
        {
            (int line, int character) = TextCoordinateConverter.GetPosition(lineStarts, diagnostic.Span.Position);
            string message = $"{filePath}({line + 1},{character + 1}) : error {diagnostic.Code}: {diagnostic.Message}";

            this.logger.Log(LogLevel.Error, message);

            // TODO: Fix this when we have diagnostic severity
            this.HasLoggedErrors = true;
        }

        public bool HasLoggedErrors { get; private set; }
    }
}