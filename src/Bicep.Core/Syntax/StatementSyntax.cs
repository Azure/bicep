// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;

namespace Bicep.Core.Syntax
{
    public abstract class StatementSyntax : DecorableSyntax
    {
        protected StatementSyntax(IEnumerable<SyntaxBase> leadingNodes) : base(leadingNodes) { }
    }
}
