// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;

namespace Bicep.Core.Workspaces;

public interface ISourceFileLookup
{
    public DiagnosticBuilder.ErrorBuilderDelegate? TryGetErrorDiagnostic(IArtifactReferenceSyntax foreignTemplateReference);

    public ISourceFile? TryGetSourceFile(IArtifactReferenceSyntax foreignTemplateReference);
}
