// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;

namespace Bicep.Core.UnitTests.Extensions;

public static class CompilationTestExtensions
{
    /// <summary>
    /// Emits and returns the ARM template compiled from the provided file URI. If a file URI is not provided, the entry point of the
    /// compilation is used by default.
    /// </summary>
    /// <param name="compilation">The compilation.</param>
    /// <param name="bicepFileUri">(Optional) The Bicep file URI to compile an ARM template from.</param>
    /// <returns>ARM template JSON string.</returns>
    public static string GetTestTemplate(this Compilation compilation, IOUri? bicepFileUri = null)
    {
        var stringBuilder = new StringBuilder();
        var stringWriter = new StringWriter(stringBuilder);

        SemanticModel? bicepFileModel;
        if (bicepFileUri is not null)
        {
            var targetFile = compilation.SourceFileGrouping.SourceFiles.OfType<BicepFile>().Single(f => f.FileHandle.Uri == bicepFileUri);
            bicepFileModel = compilation.GetSemanticModel(targetFile);
        }
        else if (compilation.SourceFileGrouping.EntryPoint is BicepFile)
        {
            bicepFileModel = compilation.GetEntrypointSemanticModel();
        }
        else
        {
            throw new InvalidOperationException($"The compilation entry point is a '{compilation.SourceFileGrouping.EntryPoint.GetType().Name}' and not a '{nameof(BicepFile)}'. An ARM template cannot be emitted.");
        }

        var emitter = new TemplateEmitter(bicepFileModel);
        emitter.Emit(stringWriter);

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Emits and returns the ARM template compiled from the params file compilation.
    /// </summary>
    /// <param name="result">The compilation.</param>
    /// <param name="bicepFileName">(Optional) The Bicep file name to compile an ARM template from. Defaults to 'main.bicep'.</param>
    /// <returns>ARM template JSON string.</returns>
    public static string GetTestTemplate(this CompilationHelper.ParamsCompilationResult result, string bicepFileName = "main.bicep")
    {
        var bicepFileHandle = result.Compilation.SourceFileGrouping.SourceFiles.Single(f => f.FileHandle.Uri.GetFileName().Equals(bicepFileName, StringComparison.OrdinalIgnoreCase)).FileHandle;
        var stringBuilder = new StringBuilder();
        var stringWriter = new StringWriter(stringBuilder);

        var model = result.Compilation.GetSemanticModel(result.Compilation.SourceFileGrouping.SourceFiles.Single(f => f.FileHandle == bicepFileHandle));

        if (model is not SemanticModel bicepFileModel)
        {
            throw new InvalidOperationException($"The file with URI '{bicepFileHandle}' is not a '{nameof(BicepFile)}'. An ARM template cannot be emitted.");
        }

        var emitter = new TemplateEmitter(bicepFileModel);
        emitter.Emit(stringWriter);

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Emits and returns the parameters file compiled from the params file compilation.
    /// </summary>
    /// <param name="result">The compilation.</param>
    /// <returns>Parameters file JSON string.</returns>
    public static string? GetTestParametersFile(this CompilationHelper.ParamsCompilationResult result)
        => result.Compilation.Emitter.Parameters().Parameters;
}
