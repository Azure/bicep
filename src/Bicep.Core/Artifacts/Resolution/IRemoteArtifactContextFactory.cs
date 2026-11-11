// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts.References;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Artifacts.Resolution;

/// <summary>
/// Gets the context for external artifact references written in the file at a location.
/// A URI that is not a file, such as an in-memory document, gets the context of the built-in configuration.
/// </summary>
public interface IRemoteArtifactContextFactory
{
    OciArtifactContext GetOciArtifactContext(IOUri referencingUri);

    TemplateSpecArtifactContext GetTemplateSpecArtifactContext(IOUri referencingUri);
}
