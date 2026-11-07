// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


namespace Bicep.Core.Artifacts
{
    public class ArtifactException : Exception
    {
        public ArtifactException(string message)
            : base(message)
        {
        }

        public ArtifactException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
