// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Common;
using Bicep.Core.Diagnostics;
using Bicep.Core.SemanticVersioning;
using Bicep.Core.Semantics;
using Bicep.Core.Text;

namespace Bicep.Core.Analyzers.Linter.Rules;

public sealed class NoConflictingVersionConstraintsRule : LinterRuleBase
{
    public new const string Code = "no-conflicting-version-constraints";

    public NoConflictingVersionConstraintsRule() : base(
        code: Code,
        description: CoreResources.NoConflictingVersionConstraintsRuleDescription,
        LinterRuleCategory.DeploymentError)
    { }

    public override string FormatMessage(params object[] values)
        => string.Format(CoreResources.NoConflictingVersionConstraintsRuleMessageFormat, values);

    public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
    {
        // This rule inspects the whole compilation's graph of local module references, so it only needs to run
        // once. Anchoring that single run to the entrypoint (rather than every file) avoids emitting the same
        // diagnostic once per file in the compilation.
        if (model.SourceFile != model.SourceFileGrouping.EntryPoint)
        {
            yield break;
        }

        (VersionRange Range, string File)? combined = null;

        foreach (var referenced in model.EnumerateAllLocalModuleModelsTransitively())
        {
            if (referenced.Configuration.Compiler.Version is not { } constraint)
            {
                // No constraint declared for this file - it doesn't narrow the combined range.
                continue;
            }

            var file = referenced.SourceFile.FileHandle.Uri.ToString();

            if (combined is not { } current)
            {
                combined = (constraint, file);
                continue;
            }

            if (!current.Range.TryGetOverlap(constraint, out var overlap))
            {
                yield return CreateDiagnosticForSpan(
                    diagnosticLevel,
                    TextSpan.TextDocumentStart,
                    current.Range.ToString(),
                    current.File,
                    constraint.ToString(),
                    file);

                yield break;
            }

            combined = (overlap, current.File);
        }
    }
}
