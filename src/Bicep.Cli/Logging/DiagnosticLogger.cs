// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Cli.Arguments;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Text;
using Microsoft.CodeAnalysis.Sarif;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Bicep.Cli.Logging;

public record DiagnosticSummary(
    bool HasErrors);

public record DiagnosticOptions(
    DiagnosticsFormat Format,
    bool SarifToStdout)
{
    public static DiagnosticOptions Default => new(
        Format: DiagnosticsFormat.Default,
        SarifToStdout: false);
}

public class DiagnosticLogger
{
    private readonly ILogger logger;
    private readonly IOContext ioContext;

    public DiagnosticLogger(ILogger logger, IOContext ioContext)
    {
        this.logger = logger;
        this.ioContext = ioContext;
    }

    public DiagnosticSummary LogDiagnostics(DiagnosticOptions options, Compilation compilation)
        => LogDiagnostics(options, compilation.GetAllDiagnosticsByBicepFile());

    public DiagnosticSummary LogDiagnostics(DiagnosticOptions options, ImmutableDictionary<BicepSourceFile, ImmutableArray<IDiagnostic>> diagnosticsByBicepFile)
    {
        switch (options.Format)
        {
            case DiagnosticsFormat.Default:
                LogDefaultDiagnostics(this.logger, diagnosticsByBicepFile);
                break;
            case DiagnosticsFormat.Sarif:
                var writer = options.SarifToStdout ? this.ioContext.Output.Writer : this.ioContext.Error.Writer;
                LogSarifDiagnostics(writer, diagnosticsByBicepFile, []);
                break;
            default:
                throw new NotImplementedException();
        }

        var hasErrors = diagnosticsByBicepFile.Values.SelectMany(x => x).Any(x => x.IsError());

        return new DiagnosticSummary(
            HasErrors: hasErrors);
    }

    internal DiagnosticSummary LogSarifDiagnostics(
        ImmutableDictionary<BicepSourceFile, ImmutableArray<IDiagnostic>> diagnosticsByBicepFile,
        ImmutableArray<(Bicep.IO.Abstraction.IOUri SourceUri, IDiagnostic Diagnostic)> diagnostics)
    {
        LogSarifDiagnostics(this.ioContext.Error.Writer, diagnosticsByBicepFile, diagnostics);

        return new(
            diagnosticsByBicepFile.Values.SelectMany(x => x).Any(x => x.IsError()) ||
            diagnostics.Any(item => item.Diagnostic.IsError()));
    }

    private static void LogDefaultDiagnostics(ILogger logger, ImmutableDictionary<BicepSourceFile, ImmutableArray<IDiagnostic>> diagnosticsByBicepFile)
    {
        foreach (var (bicepFile, diagnostics) in diagnosticsByBicepFile)
        {
            foreach (var diagnostic in diagnostics)
            {
                (var line, var character) = TextCoordinateConverter.GetPosition(bicepFile.LineStarts, diagnostic.Span.Position);

                // build a a code description link if the Uri is assigned
                var codeDescription = diagnostic.Uri == null ? string.Empty : $" [{diagnostic.Uri.AbsoluteUri}]";

                var message = $"{bicepFile.FileHandle.Uri}({line + 1},{character + 1}) : {diagnostic.Level} {diagnostic.Code}: {diagnostic.Message}{codeDescription}";

                logger.Log(ToLogLevel(diagnostic.Level), message);
            }
        }
    }

    private static void LogSarifDiagnostics(
        TextWriter writer,
        ImmutableDictionary<BicepSourceFile, ImmutableArray<IDiagnostic>> diagnosticsByBicepFile,
        ImmutableArray<(Bicep.IO.Abstraction.IOUri SourceUri, IDiagnostic Diagnostic)> additionalDiagnostics)
    {
        var results = new List<Result>();
        foreach (var (bicepFile, diagnostics) in diagnosticsByBicepFile)
        {
            foreach (var diagnostic in diagnostics)
            {
                results.Add(GetSarifDiagnostic(bicepFile, diagnostic));
            }
        }
        results.AddRange(additionalDiagnostics.Select(item => GetSarifDiagnostic(item.SourceUri, 0, 0, item.Diagnostic)));

        // Add the results from the run to the sarif log, serialize and write to stderr.
        var sarifLog = new SarifLog
        {
            Runs = new[] {
                new Run {
                    Tool = new Tool(new ToolComponent
                    {
                        Name = "bicep",
                    }, null, null),
                    Results = results,
                }
            }
        };

        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        };

        var sarifText = JsonConvert.SerializeObject(sarifLog, settings);
        writer.Write(sarifText);
        writer.Flush();
    }

    private static Result GetSarifDiagnostic(BicepSourceFile sourceFile, IDiagnostic diagnostic)
    {
        (var line, var character) = TextCoordinateConverter.GetPosition(sourceFile.LineStarts, diagnostic.Span.Position);
        return GetSarifDiagnostic(sourceFile.FileHandle.Uri, line, character, diagnostic);
    }

    private static Result GetSarifDiagnostic(
        Bicep.IO.Abstraction.IOUri sourceUri,
        int line,
        int character,
        IDiagnostic diagnostic)
    {
        // build a a code description link if the Uri is assigned
        var codeDescription = diagnostic.Uri == null ? string.Empty : $" [{diagnostic.Uri.AbsoluteUri}]";

        return new Result
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
                            Uri = sourceUri.ToUri(),
                        },
                        Region = new Region
                        {
                            StartLine = line + 1,
                            CharOffset = character + 1,
                        }
                    }
                }
            }
        };
    }

    protected static FailureLevel ToFailureLevel(DiagnosticLevel level)
        => level switch
        {
            DiagnosticLevel.Info => FailureLevel.Note,
            DiagnosticLevel.Warning => FailureLevel.Warning,
            DiagnosticLevel.Error => FailureLevel.Error,
            _ => throw new ArgumentException($"Unrecognized level {level}"),
        };

    protected static LogLevel ToLogLevel(DiagnosticLevel level)
        => level switch
        {
            DiagnosticLevel.Info => LogLevel.Information,
            DiagnosticLevel.Warning => LogLevel.Warning,
            DiagnosticLevel.Error => LogLevel.Error,
            _ => throw new ArgumentException($"Unrecognized level {level}"),
        };
}
