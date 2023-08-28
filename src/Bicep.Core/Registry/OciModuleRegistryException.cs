// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.Core.Registry
{
    public class OciModuleRegistryException : ExternalArtifactException
    {
        public OciModuleRegistryException(string message) : base(message)
        {
        }

        public OciModuleRegistryException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
