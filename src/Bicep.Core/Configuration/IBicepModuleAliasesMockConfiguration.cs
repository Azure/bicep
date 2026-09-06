// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.Configuration
{
    public interface IBicepModuleAliasesMockConfiguration
    {
        ImmutableSortedDictionary<string, OciArtifactModuleAliasMock> GetOciArtifactModuleAliasesMock();

        ResultWithDiagnosticBuilder<OciArtifactModuleAliasMock> TryGetOciArtifactModuleAliasMock(string aliasName);
    }
}
