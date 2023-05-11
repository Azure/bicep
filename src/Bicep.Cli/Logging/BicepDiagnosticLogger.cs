// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Text;
using Microsoft.CodeAnalysis.Sarif;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text.Json;

namespace Bicep.Cli.Logging
{
    /// <summary>
    /// Wraps a dotnet logger and keeps track of error history.
    /// </summary>
    public class BicepDiagnosticLogger : IDiagnosticLogger
    {
        private readonly ILogger logger;
        private readonly IOContext io;

        public BicepDiagnosticLogger(ILogger logger, IOContext io)
        {
            this.logger = logger;
            this.io = io;
            this.ErrorCount = 0;
            this.WarningCount = 0;
            sarifResults = new List<Result>();
            sarifLog = new SarifLog();
        }

        public void LogDiagnostic(Uri fileUri, IDiagnostic diagnostic, ImmutableArray<int> lineStarts)
        {
            (int line, int character) = TextCoordinateConverter.GetPosition(lineStarts, diagnostic.Span.Position);

            // build a a code description link if the Uri is assigned
            var codeDescription = diagnostic.Uri == null ? string.Empty : $" [{diagnostic.Uri.AbsoluteUri}]";
            if (Format == DiagnosticsFormat.Default) 
            {
                var message = $"{fileUri.LocalPath}({line + 1},{character + 1}) : {diagnostic.Level} {diagnostic.Code}: {diagnostic.Message}{codeDescription}";
                this.logger.Log(ToLogLevel(diagnostic.Level), message);
            }
            else if (Format == DiagnosticsFormat.Json)
            {
                sarifResults.Add(new Result()
                {
                    RuleId = diagnostic.Code,
                    Level = ToFailureLevel(diagnostic.Level),
                    Message = new Message
                    {
                        Text = $"{diagnostic.Message}{codeDescription}",
                    },
                    Locations = new[]
                    {
                        new Location
                        {
                            PhysicalLocation = new PhysicalLocation
                            {
                                ArtifactLocation = new ArtifactLocation
                                {
                                    Uri = fileUri
                                },
                                Region = new Region
                                {
                                    StartLine = line + 1,
                                    CharOffset = character + 1,
                                }
                            }
                        }
                    }
                });
            }

            // Increment counters
            if (diagnostic.Level == DiagnosticLevel.Warning) { this.WarningCount++; }
            if (diagnostic.Level == DiagnosticLevel.Error) { this.ErrorCount++; }
        }


        private enum DiagnosticsFormat
        {
            Default,
            Json
        }

        public int ErrorCount { get; private set; }

        private int WarningCount { get; set; }

        private DiagnosticsFormat Format { get; set; }

        private SarifLog sarifLog { get; set; }

        private List<Result> sarifResults { get; set; }


        public void SetupFormat(string? format)
        {
            Format = ToDiagnosticsFormat(format);
            if(Format == DiagnosticsFormat.Json)
            {
                // setup an empty run on the sarif log to add results to.
                sarifLog.Runs = new List<Run>();
                sarifLog.Runs.Add(new Run()
                {
                    Tool = new Tool(new ToolComponent()
                    {
                        Name = "bicep",
                    }, null, null)
                });
            }
        }

        public void FlushLog()
        {
            if(Format == DiagnosticsFormat.Json && sarifResults.Count > 0)
            {
                // if there are errors/warnings, add the results from the run to the sarif log, serialize and write to stderr.
                // if none are present, omit flushing the in memory log to maintain parity with how the default diagnostics are handled.
                sarifLog.Runs[0].Results = sarifResults;
                var settings = new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented
                };

                var sarifText = JsonConvert.SerializeObject(sarifLog, settings);
                io.Error.Write(sarifText);
                io.Error.Flush();
            }
        }

        private static DiagnosticsFormat ToDiagnosticsFormat(string? format)
        {
            if(format is null || (format is not null && format.Equals("default", StringComparison.OrdinalIgnoreCase)))
            {
                return DiagnosticsFormat.Default;
            }
            else if(format is not null && format.Equals("json", StringComparison.OrdinalIgnoreCase))
            {
                return DiagnosticsFormat.Json;
            }

            throw new ArgumentException($"Unrecognized diagnostics format {format}");
        }

        private static FailureLevel ToFailureLevel(DiagnosticLevel level)
            => level switch
            {
                DiagnosticLevel.Info => FailureLevel.Note,
                DiagnosticLevel.Warning => FailureLevel.Warning,
                DiagnosticLevel.Error => FailureLevel.Error,
                _ => throw new ArgumentException($"Unrecognized level {level}"),
            };

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
