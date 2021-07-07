// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Azure.Deployments.Core.Entities;
using Newtonsoft.Json;

namespace Bicep.Core.Syntax.Converters
{
    public class SyntaxConversionException : Exception
    {
        public SyntaxConversionException(string message, JTokenMetadata metadata, Exception? innerException = null)
            : base(message, innerException)
        {
            this.LineNumber = metadata.LineNumber ?? 1;
            this.LinePosition = metadata.LinePosition ?? 1;
        }

        public SyntaxConversionException(string message, IJsonLineInfo lineInfo, Exception? innerException = null)
            : base(message, innerException)
        {
            this.LineNumber = lineInfo.LineNumber;
            this.LinePosition = lineInfo.LinePosition;
        }

        public int LineNumber { get; }

        public int LinePosition { get; }
    }
}
