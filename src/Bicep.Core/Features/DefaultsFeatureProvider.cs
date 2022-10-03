// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.IO;

namespace Bicep.Core.Features;

internal class DefaultsFeatureProvider : IFeatureProvider
{
    public string CacheRootDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".bicep");

    public bool RegistryEnabled => true;

    public bool SymbolicNameCodegenEnabled => false;

    public bool ImportsEnabled => false;

    public bool ResourceTypedParamsAndOutputsEnabled => false;

    public string AssemblyVersion => ThisAssembly.AssemblyFileVersion;

    public bool SourceMappingEnabled => false;

    public bool ParamsFilesEnabled => false;
}
