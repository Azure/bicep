// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.Core.Registry
{
    public enum InvalidModuleExceptionKind
    {
        NotSpecified,
        WrongArtifactType,
        WrongModuleLayerMediaType
    }

    public class InvalidModuleException : OciModuleRegistryException
    {

        public InvalidModuleExceptionKind Kind { get; }

        public InvalidModuleException(string innerMessage, InvalidModuleExceptionKind kind = InvalidModuleExceptionKind.NotSpecified) : base($"The OCI artifact is not a valid Bicep module. {innerMessage}")
        {
            Kind = kind;
        }

        public InvalidModuleException(string innerMessage, Exception innerException, InvalidModuleExceptionKind kind = InvalidModuleExceptionKind.NotSpecified)
            : base($"The OCI artifact is not a valid Bicep module. {innerMessage}", innerException)
        {
            Kind = kind;
        }
    }
}
