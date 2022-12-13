// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Syntax
{
    /// <summary>
    /// Represents an unbound reference to a variable
    /// </summary>
    public class UnboundVariableAccessSyntax: VariableAccessSyntax
    {
        public UnboundVariableAccessSyntax(IdentifierSyntax name): base(name)
        {
        }
    }
}

