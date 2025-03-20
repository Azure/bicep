// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.SourceLink;
using Bicep.Core.Text;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer.Extensions
{
    public static class TextRangeExtensions
    {
        public static Range ToLspRange(this TextRange range) => new(range.Start.ToLspPosition(), range.End.ToLspPosition());
    }
}

