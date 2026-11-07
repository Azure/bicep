// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts;

namespace Bicep.Core.Registry
{
    public class TemplateSpecException : ArtifactException
    {
        public TemplateSpecException(string message)
            : base(message)
        {
        }

        public TemplateSpecException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
