// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Registry
{
    public enum InvalidArtifactExceptionKind
    {
        NotSpecified,
        WrongArtifactType,
        UnknownLayerMediaType,
        InvalidArtifactContents
    }

    public class InvalidArtifactException : OciArtifactRegistryException
    {

        public InvalidArtifactExceptionKind Kind { get; }

        public InvalidArtifactException(string innerMessage, InvalidArtifactExceptionKind kind = InvalidArtifactExceptionKind.NotSpecified) : base($"The OCI artifact is not a valid Bicep artifact. {innerMessage}")
        {
            Kind = kind;
        }

        public InvalidArtifactException(string innerMessage, Exception innerException, InvalidArtifactExceptionKind kind = InvalidArtifactExceptionKind.NotSpecified)
            : base($"The OCI artifact is not a valid Bicep artifact. {innerMessage}", innerException)
        {
            Kind = kind;
        }
    }
}
