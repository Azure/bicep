// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


namespace Bicep.Core.Artifacts.Registries
{
    public class OciArtifactRegistryException : ExternalArtifactException
    {
        public OciArtifactRegistryException(string message) : base(message)
        {
        }

        public OciArtifactRegistryException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
