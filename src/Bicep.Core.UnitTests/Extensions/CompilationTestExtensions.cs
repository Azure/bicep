// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;

namespace Bicep.Core.UnitTests.Extensions;

public static class CompilationTestExtensions
{
    public static (bool success, IDictionary<Uri, ImmutableArray<IDiagnostic>> diagnosticsByFile) GetSuccessAndDiagnosticsByBicepFile(this Compilation compilation)
    {
        var diagnosticsByFile = compilation.GetAllDiagnosticsByBicepFile().ToDictionary(kvp => kvp.Key.Uri, kvp => kvp.Value);
        var success = diagnosticsByFile.Values.SelectMany(x => x).All(d => !d.IsError());

        return (success, diagnosticsByFile);
    }

    public static string GetTestTemplate(this Compilation compilation)
    {
        var stringBuilder = new StringBuilder();
        var stringWriter = new StringWriter(stringBuilder);

        var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel());
        emitter.Emit(stringWriter);

        return stringBuilder.ToString();
    }
}
