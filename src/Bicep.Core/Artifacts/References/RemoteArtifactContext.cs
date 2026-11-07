// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Artifacts.References;

/// <summary>
/// The settings used to reach an external artifact in its registry and in the local cache. They come from the configuration
/// of the file that references the artifact, so the same artifact reached from folders with different settings is a
/// different reference. Each kind of external artifact has its own context type with the settings it uses.
/// </summary>
/// <param name="Cloud">The cloud used to authenticate to the registry.</param>
/// <param name="CacheRootDirectory">The root of the local artifact cache.</param>
public abstract record RemoteArtifactContext(IBicepCloudConfiguration Cloud, IDirectoryHandle CacheRootDirectory);
