// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.Core.Registry
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
