// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.Syntax
{
    public abstract class IdentifierSyntaxBase : SyntaxBase
    {
        public abstract string IdentifierName { get; }
    }
}