// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.SourceLink;
using Bicep.Core.Text;
using Position = OmniSharp.Extensions.LanguageServer.Protocol.Models.Position;

namespace Bicep.LanguageServer.Extensions
{
    public static class TextPositionExtensions
    {
        public static Position ToLspPosition(this TextPosition position) => new(position.Line, position.Character);
    }
}

