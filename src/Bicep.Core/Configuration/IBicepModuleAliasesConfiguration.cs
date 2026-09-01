// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.Configuration
{
    public interface IBicepModuleAliasesConfiguration
    {
        ImmutableSortedDictionary<string, OciArtifactModuleAlias> GetOciArtifactModuleAliases();

        ImmutableSortedDictionary<string, TemplateSpecModuleAlias> GetTemplateSpecModuleAliases();

        ResultWithDiagnosticBuilder<OciArtifactModuleAlias> TryGetOciArtifactModuleAlias(string aliasName);

        ResultWithDiagnosticBuilder<TemplateSpecModuleAlias> TryGetTemplateSpecModuleAlias(string aliasName);
    }
}
