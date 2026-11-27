// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Analyzers.Linter.Rules;

public sealed class NoLoadOfBuildOutputRule : LinterRuleBase
{
    public new const string Code = "no-load-of-build-output";

    private static readonly string[] LoadFunctionNames =
    [
        "loadTextContent",
        "loadFileAsBase64",
        "loadJsonContent",
        "loadYamlContent",
    ];

    public NoLoadOfBuildOutputRule() : base(
        code: Code,
        description: CoreResources.NoLoadOfBuildOutputRule_Description,
        category: LinterRuleCategory.BestPractice)
    { }

    public override string FormatMessage(params object[] values)
        => string.Format(CoreResources.NoLoadOfBuildOutputRule_MessageFormat, values);

    public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
    {
        var sourceUri = model.SourceFile.FileHandle.Uri;
        if (!sourceUri.HasBicepExtension())
        {
            yield break;
        }

        // The default output of `bicep build foo.bicep` is `foo.json` in the same directory.
        var defaultOutputPath = string.Concat(sourceUri.Path.AsSpan(0, sourceUri.Path.Length - LanguageConstants.LanguageFileExtension.Length), ".json");

        foreach (var functionName in LoadFunctionNames)
        {
            foreach (var call in SemanticModelHelper.GetFunctionsByName(model, SystemNamespaceType.BuiltInName, functionName, model.Root.Syntax))
            {
                if (call.Arguments.FirstOrDefault()?.Expression is not StringSyntax pathSyntax ||
                    pathSyntax.TryGetLiteralValue() is not { } literalPath ||
                    !RelativePath.TryCreate(literalPath).IsSuccess(out var relativePath) ||
                    !model.SourceFile.TryLoadAuxiliaryFile(relativePath).IsSuccess(out var loadedFile))
                {
                    continue;
                }

                if (string.Equals(loadedFile.Uri.Path, defaultOutputPath, sourceUri.PathComparison))
                {
                    yield return CreateDiagnosticForSpan(diagnosticLevel, call.Arguments[0].Span, literalPath);
                }
            }
        }
    }
}
