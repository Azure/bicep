// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Commands;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Documentation;
using Bicep.Core.Exceptions;
using Bicep.Core.Features;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Services;

public abstract record DocsRenderResult(
    IOUri SourceUri,
    Compilation? CompilationResult,
    IDiagnostic? DocumentationDiagnostic)
{
    public sealed record Succeeded(IOUri SourceUri, Compilation Compilation, string Contents)
        : DocsRenderResult(SourceUri, Compilation, null);

    public sealed record Failed(
        IOUri SourceUri,
        Compilation? Compilation = null,
        IDiagnostic? DocumentationDiagnostic = null)
        : DocsRenderResult(SourceUri, Compilation, DocumentationDiagnostic);
}

public class DocsCommandRunner(
    ILogger logger,
    IOContext io,
    DiagnosticLogger diagnosticLogger,
    BicepCompiler compiler,
    IBicepDocumentationGenerator documentationGenerator)
{
    public async Task<DocsRenderResult> RenderAsync(
        IOUri inputUri,
        IOUri? templateFile,
        IOUri? templateRoot,
        IReadOnlyDictionary<string, string> customValues,
        bool noRestore,
        DiagnosticsFormat? diagnosticsFormat,
        ActiveSourceFileSet? workspace = null,
        bool logExperimentalWarning = true,
        bool logDiagnostics = true,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Compilation compilation;
        try
        {
            compilation = await compiler.CreateCompilation(inputUri, workspace, skipRestore: noRestore);
            workspace?.UpsertSourceFiles(compilation.SourceFileGrouping.SourceFiles);
        }
        catch (BicepException exception)
        {
            if (diagnosticsFormat is not DiagnosticsFormat.Sarif)
            {
                await io.Error.Writer.WriteLineAsync(exception.Message);
            }

            return new DocsRenderResult.Failed(
                inputUri,
                DocumentationDiagnostic: DocsCommand.CreateDiagnostic(DocsCommand.InputFailureCode, exception.Message));
        }

        var shouldLogExperimentalWarning = logExperimentalWarning && diagnosticsFormat is not DiagnosticsFormat.Sarif;
        if (shouldLogExperimentalWarning)
        {
            CommandHelper.LogExperimentalWarning(logger, compilation);
        }

        var hasErrors = logDiagnostics
            ? diagnosticLogger.LogDiagnostics(ArgumentHelper.GetDiagnosticOptions(diagnosticsFormat), compilation).HasErrors
            : compilation.GetAllDiagnosticsByBicepFile().Values.SelectMany(diagnostics => diagnostics).Any(diagnostic => diagnostic.IsError());
        if (hasErrors)
        {
            return new DocsRenderResult.Failed(inputUri, compilation);
        }

        if (shouldLogExperimentalWarning)
        {
            logger.LogWarning(string.Format(
                CliResources.ExperimentalFeaturesDisclaimerMessage,
                "docs"));
        }

        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var options = new BicepDocumentationGenerationOptions(templateFile, templateRoot, customValues);
            return new DocsRenderResult.Succeeded(
                inputUri,
                compilation,
                documentationGenerator.Generate(compilation, options, cancellationToken));
        }
        catch (BicepDocumentationException exception)
        {
            if (diagnosticsFormat is not DiagnosticsFormat.Sarif)
            {
                await io.Error.Writer.WriteLineAsync(exception.Message);
            }

            return new DocsRenderResult.Failed(
                inputUri,
                compilation,
                DocsCommand.CreateDiagnostic(DocsCommand.RenderFailureCode, exception.Message));
        }
    }
}
