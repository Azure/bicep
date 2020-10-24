// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
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

        public void LogDiagnostic(Uri fileUri, Diagnostic diagnostic, ImmutableArray<int> lineStarts)
        {
            (int line, int character) = TextCoordinateConverter.GetPosition(lineStarts, diagnostic.Span.Position);
            string message = $"{fileUri.LocalPath}({line + 1},{character + 1}) : {diagnostic.Level} {diagnostic.Code}: {diagnostic.Message}";

            this.logger.Log(ToLogLevel(diagnostic.Level), message);

            this.HasLoggedErrors |= diagnostic.Level == DiagnosticLevel.Error;
        }

        public bool HasLoggedErrors { get; private set; }

        private static LogLevel ToLogLevel(DiagnosticLevel level)
            => level switch {
                DiagnosticLevel.Info => LogLevel.Information,
                DiagnosticLevel.Warning => LogLevel.Warning,
                DiagnosticLevel.Error => LogLevel.Error,
                _ => throw new ArgumentException($"Unrecognized level {level}"),
            };

    }
}
