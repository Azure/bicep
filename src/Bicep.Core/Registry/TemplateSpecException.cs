// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.Core.Registry
{
    public class TemplateSpecException : Exception
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
