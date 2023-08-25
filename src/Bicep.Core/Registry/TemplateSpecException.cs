// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.Core.Registry
{
    public class TemplateSpecException : ExternalArtifactException
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
