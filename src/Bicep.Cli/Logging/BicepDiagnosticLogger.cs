// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Text;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Immutable;

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
            this.ErrorCount = 0;
            this.WarningCount = 0;
        }

        public void LogDiagnostic(Uri fileUri, IDiagnostic diagnostic, ImmutableArray<int> lineStarts)
        {
            (int line, int character) = TextCoordinateConverter.GetPosition(lineStarts, diagnostic.Span.Position);

            // build a a code description link if the Uri is assigned
            var codeDescription = diagnostic.Uri == null ? string.Empty : $" [{diagnostic.Uri.AbsoluteUri}]";

            var message = $"{fileUri.LocalPath}({line + 1},{character + 1}) : {diagnostic.Level} {diagnostic.Code}: {diagnostic.Message}{codeDescription}";

            this.logger.Log(ToLogLevel(diagnostic.Level), message);

            // Increment counters
            if (diagnostic.Level == DiagnosticLevel.Warning) { this.WarningCount++; }
            if (diagnostic.Level == DiagnosticLevel.Error) { this.ErrorCount++; }
        }

        public int ErrorCount { get; private set; }
        
        private int WarningCount { get; set; }

        private static LogLevel ToLogLevel(DiagnosticLevel level)
            => level switch
            {
                DiagnosticLevel.Info => LogLevel.Information,
                DiagnosticLevel.Warning => LogLevel.Warning,
                DiagnosticLevel.Error => LogLevel.Error,
                _ => throw new ArgumentException($"Unrecognized level {level}"),
            };

    }
}
