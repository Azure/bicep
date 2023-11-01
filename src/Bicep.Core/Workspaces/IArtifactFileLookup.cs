// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Utils;

namespace Bicep.Core.Workspaces;

public interface IArtifactFileLookup
{
    public ResultWithDiagnostic<ISourceFile> TryGetSourceFile(IArtifactReferenceSyntax foreignTemplateReference);

    public ResultWithDiagnostic<Uri> TryGetProviderFileUri(IArtifactReferenceSyntax foreignTemplateReference);
}
