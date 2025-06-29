// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Text;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer.Extensions
{
    public static class SourceCodeRangeExtensions
    {
        public static Range ToRange(this TextRange range) => new(range.Start.ToPosition(), range.End.ToPosition());
    }
}

