// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.SourceLink;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer.Extensions
{
    public static class SourceCodeRangeExtensions
    {
        public static Range ToRange(this SourceCodeRange range) => new(range.Start.ToPosition(), range.End.ToPosition());
    }
}

