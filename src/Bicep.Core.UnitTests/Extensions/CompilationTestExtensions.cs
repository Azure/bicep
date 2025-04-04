// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;

namespace Bicep.Core.UnitTests.Extensions;

public static class CompilationTestExtensions
{
    public static (bool success, IDictionary<Uri, ImmutableArray<IDiagnostic>> diagnosticsByFile) GetSuccessAndDiagnosticsByBicepFile(this Compilation compilation)
    {
        var diagnosticsByFile = compilation.GetAllDiagnosticsByBicepFile().ToDictionary(kvp => kvp.Key.Uri, kvp => kvp.Value);
        var success = diagnosticsByFile.Values.SelectMany(x => x).All(d => !d.IsError());

        return (success, diagnosticsByFile);
    }

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

        var sourceFileModel = bicepFileUri switch
        {
            not null when compilation.SourceFileGrouping.SourceFiles.OfType<BicepFile>().SingleOrDefault(f => f.Uri == bicepFileUri) is { } bicepFile
                => compilation.GetSemanticModel(bicepFile),
            _ when compilation.SourceFileGrouping.EntryPoint is BicepFile
                => compilation.GetEntrypointSemanticModel(),
            _ => throw new InvalidOperationException($"The compilation entry point is a '{compilation.SourceFileGrouping.EntryPoint.GetType().Name}' and not a '{nameof(BicepFile)}'. An ARM template cannot be emitted.")
        };

        var emitter = new TemplateEmitter(sourceFileModel);
        emitter.Emit(stringWriter);

        return stringBuilder.ToString();
    }
}
