// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.Core.Registry
{
    public class ExternalModuleException : Exception
    {
        public ExternalModuleException(string message)
            : base(message)
        {
        }

        public ExternalModuleException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
