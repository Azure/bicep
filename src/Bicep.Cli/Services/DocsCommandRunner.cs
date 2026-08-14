// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.Documentation;
using Bicep.Core.Exceptions;
using Bicep.Core.Features;
using Bicep.Core.Semantics;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Services;

public record DocsRenderResult(bool Success, string? Contents);

public class DocsCommandRunner(
    ILogger logger,
    IOContext io,
    DiagnosticLogger diagnosticLogger,
    BicepCompiler compiler,
    IFeatureProviderFactory featureProviderFactory,
    IBicepDocumentationGenerator documentationGenerator)
{
    public async Task<DocsRenderResult> RenderAsync(
        IOUri inputUri,
        BicepDocumentationPreset preset,
        IOUri? templateFile,
        IOUri? templateRoot,
        IReadOnlyDictionary<string, string> customValues,
        bool noRestore,
        DiagnosticsFormat? diagnosticsFormat)
    {
        if (!featureProviderFactory.GetFeatureProvider(inputUri).DocsGenerationEnabled)
        {
            await io.Error.Writer.WriteLineAsync(
                $"The '{nameof(Bicep.Core.Configuration.ExperimentalFeaturesEnabled.DocsGeneration)}' experimental feature must be enabled for \"{inputUri}\".");
            return new(false, null);
        }

        Compilation compilation;
        try
        {
            compilation = await compiler.CreateCompilation(inputUri, skipRestore: noRestore);
        }
        catch (BicepException exception)
        {
            await io.Error.Writer.WriteLineAsync(exception.Message);
            return new(false, null);
        }

        CommandHelper.LogExperimentalWarning(logger, compilation);

        var summary = diagnosticLogger.LogDiagnostics(ArgumentHelper.GetDiagnosticOptions(diagnosticsFormat), compilation);
        if (summary.HasErrors)
        {
            return new(false, null);
        }

        logger.LogWarning(string.Format(
            CliResources.ExperimentalFeaturesDisclaimerMessage,
            nameof(Bicep.Core.Configuration.ExperimentalFeaturesEnabled.DocsGeneration)));

        try
        {
            var options = new BicepDocumentationGenerationOptions(preset, templateFile, templateRoot, customValues);
            return new(true, documentationGenerator.Generate(compilation, options));
        }
        catch (BicepDocumentationException exception)
        {
            await io.Error.Writer.WriteLineAsync(exception.Message);
            return new(false, null);
        }
    }
}
