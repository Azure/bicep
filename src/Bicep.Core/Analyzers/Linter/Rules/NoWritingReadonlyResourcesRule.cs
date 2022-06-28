// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.TypeSystem;
using System;
using System.Collections.Generic;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class NoWritingReadonlyResourcesRule : LinterRuleBase
    {
        public new const string Code = "no-writing-readonly-resources";

        public NoWritingReadonlyResourcesRule() : base(
            code: Code,
            description: CoreResources.NoWritingReadonlyResourcesDescription,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"))
        { }

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            foreach (var r in model.DeclaredResources)
            {
                if (!r.IsExistingResource && IsReadonlyResource(r))
                {
                    yield return CreateDiagnosticForSpan(r.NameSyntax.Span, r.Type.Name);
                }
            }
        }

        public override string FormatMessage(params object[] values)
            => string.Format(CoreResources.NoWritingReadonlyResourcesMessageFormat, values);

        private static bool IsReadonlyResource(DeclaredResourceMetadata r)
            => (r.Type.Flags & ResourceFlags.ReadOnly) == ResourceFlags.ReadOnly;
    }
}
