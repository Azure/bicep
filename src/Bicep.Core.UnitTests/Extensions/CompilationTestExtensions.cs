// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.UnitTests.Utils;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

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
    public static string GetTestTemplate(this Compilation compilation, Uri? bicepFileUri = null)
    {
        var stringBuilder = new StringBuilder();
        var stringWriter = new StringWriter(stringBuilder);

        SemanticModel? bicepFileModel;
        if (bicepFileUri is not null)
        {
            var targetFile = compilation.SourceFileGrouping.SourceFiles.OfType<BicepFile>().Single(f => f.Uri == bicepFileUri);
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
    /// <param name="bicepFileUri">The Bicep file URI to generate the template from. If not provided, finding a "main.bicep" is attempted.</param>
    /// <returns>ARM template JSON string.</returns>
    public static string GetTestTemplate(this CompilationHelper.ParamsCompilationResult result, Uri? bicepFileUri = null)
    {
        bicepFileUri ??= result.Compilation.SourceFileGrouping.SourceFiles.First(f => f.Uri.AbsoluteUri.EndsWithInsensitively("/main.bicep")).Uri;

        var stringBuilder = new StringBuilder();
        var stringWriter = new StringWriter(stringBuilder);

        var model = result.Compilation.GetSemanticModel(result.Compilation.SourceFileGrouping.SourceFiles.Single(f => f.Uri == bicepFileUri));

        if (model is not SemanticModel bicepFileModel)
        {
            throw new InvalidOperationException($"The file with URI '{bicepFileUri}' is not a '{nameof(BicepFile)}'. An ARM template cannot be emitted.");
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
