// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Syntax;

namespace Bicep.Core.Navigation;

public interface IForeignTemplateReference
{
    public SyntaxBase ReferenceSourceSyntax { get; }

    public StringSyntax? TryGetPath();
}
