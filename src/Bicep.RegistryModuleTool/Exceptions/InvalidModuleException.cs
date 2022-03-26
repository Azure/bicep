// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.RegistryModuleTool.Exceptions
{
    public class InvalidModuleException : Exception
    {
        public InvalidModuleException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}
