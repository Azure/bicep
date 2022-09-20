// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Bicep.RoslynAnalyzers
{
    public class GeneratorMethodContext
    {
        public MethodDeclarationSyntax MethodSyntax { get; private set; }

        public INamedTypeSymbol ClassSymbol { get; private set; }

        public GeneratorMethodContext(MethodDeclarationSyntax methodSyntax, INamedTypeSymbol classSymbol)
        {
            this.MethodSyntax = methodSyntax;
            this.ClassSymbol = classSymbol;
        }
    }
}
