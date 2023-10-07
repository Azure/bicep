// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.Core.Registry
{
    public enum InvalidArtifactExceptionKind
    {
        NotSpecified,
        WrongArtifactType,
        WrongModuleLayerMediaType
    }

    public class InvalidArtifactException : OciModuleRegistryException
    {

        public InvalidArtifactExceptionKind Kind { get; }

        public InvalidArtifactException(string innerMessage, InvalidArtifactExceptionKind kind = InvalidArtifactExceptionKind.NotSpecified) : base($"The OCI artifact is not a valid Bicep module. {innerMessage}")
        {
            Kind = kind;
        }

        public InvalidArtifactException(string innerMessage, Exception innerException, InvalidArtifactExceptionKind kind = InvalidArtifactExceptionKind.NotSpecified)
            : base($"The OCI artifact is not a valid Bicep module. {innerMessage}", innerException)
        {
            Kind = kind;
        }
    }
}
