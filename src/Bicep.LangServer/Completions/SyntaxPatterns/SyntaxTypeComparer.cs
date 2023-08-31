// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Bicep.LanguageServer.Completions.SyntaxPatterns
{
    public class SyntaxTypeComparer : IEqualityComparer<SyntaxBase?>
    {
        public bool Equals(SyntaxBase? x, SyntaxBase? y)
        {
            if (x is null && y is null)
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            if (x is Token tokenX && y is Token tokenY)
            {
                return tokenX.Type == tokenY.Type;
            }

            return x.GetType() == y.GetType();
        }

        public int GetHashCode([DisallowNull] SyntaxBase syntax)
        {
            var hashCode = syntax.GetType().GetHashCode();

            if (syntax is Token token)
            {
                hashCode ^= token.Type.GetHashCode();
            }

            return hashCode;
        }
    }
}
