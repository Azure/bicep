// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Newtonsoft.Json;

namespace Bicep.Core.Syntax.Converters
{
    public class SyntaxConversionException : Exception
    {
        public SyntaxConversionException(string message, IJsonLineInfo jsonLineInfo, Exception? innerException = null)
            : base(message, innerException)
        {
            this.LineNumber = jsonLineInfo.LineNumber;
            this.LinePosition = jsonLineInfo.LinePosition;
        }

        public int LineNumber { get; }

        public int LinePosition { get; }
    }
}
