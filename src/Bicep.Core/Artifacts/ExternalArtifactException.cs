// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


namespace Bicep.Core.Artifacts
{
    public class ExternalArtifactException : Exception
    {
        public ExternalArtifactException(string message)
            : base(message)
        {
        }

        public ExternalArtifactException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
