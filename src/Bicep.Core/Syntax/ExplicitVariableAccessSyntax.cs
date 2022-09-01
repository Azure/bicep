// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Syntax
{
    /// <summary>
    /// Represents an explicit reference to a variable
    /// </summary>
    public class ExplicitVariableAccessSyntax: VariableAccessSyntax
    {
        public ExplicitVariableAccessSyntax(IdentifierSyntax name): base(name)
        {
        }
    }
}

