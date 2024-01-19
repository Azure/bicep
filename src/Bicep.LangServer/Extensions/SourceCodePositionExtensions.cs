// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.SourceCode;
using Position = OmniSharp.Extensions.LanguageServer.Protocol.Models.Position;

namespace Bicep.LanguageServer.Extensions
{
    public static class SourceCodePositionExtensions
    {
        public static Position ToPosition(this SourceCodePosition position) => new(position.Line, position.Column);
    }
}

