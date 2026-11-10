// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry.Oci;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Registry;

/// <summary>
/// Gets the context for external artifact references written in the file at a location.
/// A URI that is not a file, such as an in-memory document, gets the context of the built-in configuration.
/// </summary>
public interface IExternalArtifactContextFactory
{
    OciArtifactContext GetOciArtifactContext(IOUri referencingUri);

    TemplateSpecArtifactContext GetTemplateSpecArtifactContext(IOUri referencingUri);
}
