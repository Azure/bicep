// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Providers;

namespace Bicep.Core.Workspaces;

public interface IArtifactFileLookup
{
    public ResultWithDiagnostic<ISourceFile> TryGetSourceFile(IArtifactReferenceSyntax foreignTemplateReference);

    public ResultWithDiagnostic<ResourceTypesProviderDescriptor> TryGetProviderDescriptor(BicepSourceFile file, ProviderDeclarationSyntax providerDeclarationSyntax);
}
