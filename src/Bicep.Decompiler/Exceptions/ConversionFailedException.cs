// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using System;

namespace Bicep.Decompiler.Exceptions
{
    public class ConversionFailedException : Exception
    {
        public ConversionFailedException(string message, IJsonLineInfo jsonLineInfo)
            : base(FormatMessage(message, jsonLineInfo))
        {
        }

        private static string FormatMessage(string message, IJsonLineInfo jsonLineInfo)
            => $"[{jsonLineInfo.LineNumber}:{jsonLineInfo.LinePosition}]: {message}";
    }
}