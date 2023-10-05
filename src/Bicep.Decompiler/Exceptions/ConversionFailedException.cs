// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Newtonsoft.Json;

namespace Bicep.Decompiler.Exceptions
{
    public class ConversionFailedException : Exception
    {
        public ConversionFailedException(string message, IJsonLineInfo jsonLineInfo, Exception? innerException = null)
            : base(FormatMessage(message, jsonLineInfo), innerException)
        {
        }

        private static string FormatMessage(string message, IJsonLineInfo jsonLineInfo)
            => $"[{jsonLineInfo.LineNumber}:{jsonLineInfo.LinePosition}]: {message}";
    }
}
