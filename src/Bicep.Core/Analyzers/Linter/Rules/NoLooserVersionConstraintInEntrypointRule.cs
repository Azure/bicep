// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Common;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Text;

namespace Bicep.Core.Analyzers.Linter.Rules;

public sealed class NoLooserVersionConstraintInEntrypointRule : LinterRuleBase
{
    public new const string Code = "no-loose-entrypoint-version";

    public NoLooserVersionConstraintInEntrypointRule() : base(
        code: Code,
        description: CoreResources.NoLooserVersionConstraintInEntrypointRule_Description,
        LinterRuleCategory.DeploymentError)
    { }

    public override string FormatMessage(params object[] values)
        => string.Format(CoreResources.NoLooserVersionConstraintInEntrypointRule_MessageFormat, values);

    public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
    {
        // This rule inspects the whole compilation's graph of local module references, so it only needs to run
        // once. Anchoring that single run to the entrypoint (rather than every file) avoids emitting the same
        // diagnostic once per file in the compilation.
        if (model.SourceFile != model.SourceFileGrouping.EntryPoint)
        {
            yield break;
        }

        if (model.Configuration.Compiler.Version is not { } entrypointConstraint)
        {
            // The entrypoint declares no "bicep.version" constraint at all, which is the loosest constraint
            // possible - it permits every version. If any referenced file declares a real constraint, the
            // entrypoint is unavoidably looser than it, so an external tool that only resolves the entrypoint's
            // config could pick a version incompatible with that referenced file.
            var firstConstrained = model.EnumerateAllLocalModuleModelsTransitively()
                .FirstOrDefault(referenced => referenced.Configuration.Compiler.Version is not null);

            if (firstConstrained is not null)
            {
                yield return CreateDiagnosticForSpan(
                    diagnosticLevel,
                    TextSpan.TextDocumentStart,
                    "none specified",
                    firstConstrained.Configuration.Compiler.Version!.ToString(),
                    firstConstrained.SourceFile.FileHandle.Uri.ToString());
            }

            yield break;
        }

        foreach (var referenced in model.EnumerateAllLocalModuleModelsTransitively())
        {
            if (referenced.Configuration.Compiler.Version is not { } referencedConstraint)
            {
                continue;
            }

            if (!entrypointConstraint.IsSubsetOf(referencedConstraint))
            {
                // The entrypoint's constraint permits at least one version that the referenced file's constraint
                // forbids - an external tool that only resolves the entrypoint's constraint could pick a version
                // that's incompatible with this referenced file.
                yield return CreateDiagnosticForSpan(
                    diagnosticLevel,
                    TextSpan.TextDocumentStart,
                    entrypointConstraint.ToString(),
                    referencedConstraint.ToString(),
                    referenced.SourceFile.FileHandle.Uri.ToString());

                yield break;
            }
        }
    }
}
