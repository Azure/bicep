// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class SecureParamsInParametersFileRule : LinterRuleBase
    {
        public new const string Code = "secure-params-in-parameters-file";

        public SecureParamsInParametersFileRule() : base(
            code: Code,
            description: CoreResources.SecureParamsInParametersFileRule_Description,
            LinterRuleCategory.Security)
        { }

        public override string FormatMessage(params object[] values)
            => string.Format(CoreResources.SecureParamsInParametersFileRule_MessageFormat, values);

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            if (model.SourceFile.FileKind != BicepSourceFileKind.ParamsFile)
            {
                yield break;
            }

            foreach (var assignment in model.Root.ParameterAssignments)
            {
                // skip if the target param is itself secure (secure -> secure is fine)
                if (model.TryGetParameterMetadata(assignment) is not { } targetMetadata ||
                     IsSecure(targetMetadata))
                {
                    continue;
                }

                // does the assigned value transitively reference a secure parameter
                var secureRefs = model.Binder.GetReferencedSymbolClosureFor(assignment)
                    .OfType<ParameterAssignmentSymbol>()
                    .Where(p => model.TryGetParameterMetadata(p) is { } metadata && IsSecure(metadata))
                    .ToArray();

                if (secureRefs.Length > 0)
                {
                    yield return CreateDiagnosticForSpan(
                        diagnosticLevel,
                        assignment.DeclaringParameterAssignment.Value.Span,
                        assignment.Name,
                        string.Join(", ", secureRefs.Select(p => $"'{p.Name}'")));
                }
            }
        }

        private static bool IsSecure(Semantics.Metadata.ParameterMetadata targetMetadata)
        {
            return TypeHelper.IsOrContainsSecureType(targetMetadata.TypeReference.Type);
        }
    }
}