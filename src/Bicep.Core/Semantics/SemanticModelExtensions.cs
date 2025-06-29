// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;

namespace Bicep.Core.Semantics;

public static class SemanticModelExtensions
{
    public static ResultWithDiagnostic<ISemanticModel> TryGetReferencedModel(this SemanticModel model, IArtifactReferenceSyntax referenceSyntax)
        => referenceSyntax.TryGetReferencedModel(model.SourceFileGrouping, model.ModelLookup);
}
