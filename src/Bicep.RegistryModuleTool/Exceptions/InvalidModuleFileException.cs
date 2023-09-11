// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.RegistryModuleTool.Exceptions
{
    public class InvalidModuleFileException : Exception
    {
        public InvalidModuleFileException(string message)
            : base(NormalizeLineEndings(message))
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
