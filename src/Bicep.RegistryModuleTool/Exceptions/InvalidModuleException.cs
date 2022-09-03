// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.RegistryModuleTool.Exceptions
{
    public class InvalidModuleException : Exception
    {
        public InvalidModuleException(string message, Exception? innerException = null)
            : base(NormalizeLineEndings(message), innerException)
        {
        }

        public static string NormalizeLineEndings(string message)
        {
            // Normalize the message to make it always end with a new line.
            var normalizedMessage = message.ReplaceLineEndings();

            if (!normalizedMessage.EndsWith(Environment.NewLine))
            {
                normalizedMessage = $"{normalizedMessage}{Environment.NewLine}";
            }

            return normalizedMessage;
        }
    }
}
