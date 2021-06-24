// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Newtonsoft.Json;

namespace Bicep.Core.Syntax.Converters
{
    public class SyntaxConversionException : Exception
    {
        public SyntaxConversionException(string message, IJsonLineInfo jsonLineInfo, Exception? innerException = null)
            : base(FormatMessage(message, jsonLineInfo), innerException)
        {
            this.LineNumber = jsonLineInfo.LineNumber;
            this.LinePosition = jsonLineInfo.LinePosition;
        }

        public int LineNumber { get; }

        public int LinePosition { get; }

        private static string FormatMessage(string message, IJsonLineInfo jsonLineInfo) =>
            $"[{jsonLineInfo.LineNumber}:{jsonLineInfo.LinePosition}]: {message}";
    }
}
