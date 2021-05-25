// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Navigation;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.LanguageServer.Extensions;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Utils
{
    public static class PositionHelper
    {
        public static Position GetPosition(ImmutableArray<int> lineStarts, in int offset)
        {
            (int line, int character) = TextCoordinateConverter.GetPosition(lineStarts, offset);
            return new Position(line, character);
        }

        public static int GetOffset(ImmutableArray<int> lineStarts, Position position) => TextCoordinateConverter.GetOffset(lineStarts, position.Line, position.Character);

        public static Range GetNameRange(ImmutableArray<int> lineStarts, SyntaxBase syntax)
            => GetNameSyntax(syntax).ToRange(lineStarts);

        public static SyntaxBase GetNameSyntax(SyntaxBase syntax)
            => syntax switch {
                INamedDeclarationSyntax x => x.Name,
                ISymbolReference x => x.Name,
                PropertyAccessSyntax x => x.PropertyName,
                ObjectPropertySyntax x => x.Key,
                // it's something else - fallback to syntax node's span
                _ => syntax,
            };
    }
}

