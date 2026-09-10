// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.Configuration;
using Bicep.Core.Documentation;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Services;

public abstract record DocsRenderResult(
    IOUri SourceUri,
    Compilation Compilation)
{
    public sealed record Succeeded(
        IOUri SourceUri,
        Compilation Compilation,
        IBicepConfiguration Configuration,
        string Contents)
        : DocsRenderResult(SourceUri, Compilation);

    public sealed record Failed(
        IOUri SourceUri,
        Compilation Compilation)
        : DocsRenderResult(SourceUri, Compilation);
}

public class DocsCommandRunner(
    ILogger logger,
    DiagnosticLogger diagnosticLogger,
    BicepCompiler compiler,
    IBicepDocumentationGenerator documentationGenerator,
    DocsGenerationOptionsResolver optionsResolver)
{
    public async Task<DocsRenderResult> RenderAsync(
        IOUri inputUri,
        IReadOnlyDictionary<string, string> customValues,
        bool noRestore,
        DiagnosticsFormat? diagnosticsFormat,
        ActiveSourceFileSet workspace,
        bool logExperimentalWarning = true,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var compilation = await compiler.CreateCompilation(inputUri, workspace, skipRestore: noRestore);
        workspace.UpsertSourceFiles(compilation.SourceFileGrouping.SourceFiles);

        var shouldLogExperimentalWarning = logExperimentalWarning && diagnosticsFormat is not DiagnosticsFormat.Sarif;
        if (shouldLogExperimentalWarning)
        {
            CommandHelper.LogExperimentalWarning(logger, compilation);
        }

        if (diagnosticLogger.LogDiagnostics(ArgumentHelper.GetDiagnosticOptions(diagnosticsFormat), compilation).HasErrors)
        {
            return new DocsRenderResult.Failed(inputUri, compilation);
        }

        if (shouldLogExperimentalWarning)
        {
            logger.LogWarning(string.Format(
                CliResources.ExperimentalFeaturesDisclaimerMessage,
                "docs"));
        }

        var configuration = compilation.GetEntrypointSemanticModel().Configuration;
        var options = optionsResolver.Resolve(configuration, customValues);
        return new DocsRenderResult.Succeeded(
            inputUri,
            compilation,
            configuration,
            documentationGenerator.Generate(compilation, options, cancellationToken));
    }
}
