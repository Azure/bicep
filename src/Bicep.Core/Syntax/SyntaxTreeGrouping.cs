// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.Syntax
{
    public class SyntaxTreeGrouping
    {
        public SyntaxTree EntryPoint { get; }

        public ImmutableHashSet<SyntaxTree> SyntaxTrees { get; }

        public ImmutableDictionary<ModuleDeclarationSyntax, SyntaxTree> ModuleLookup { get; }

        public ImmutableDictionary<ModuleDeclarationSyntax, DiagnosticBuilder.ErrorBuilderDelegate> ModuleFailureLookup { get; }

        public SyntaxTreeGrouping(SyntaxTree entryPoint, ImmutableHashSet<SyntaxTree> syntaxTrees, ImmutableDictionary<ModuleDeclarationSyntax, SyntaxTree> moduleLookup, ImmutableDictionary<ModuleDeclarationSyntax, DiagnosticBuilder.ErrorBuilderDelegate> moduleFailureLookup)
        {
            EntryPoint = entryPoint;
            SyntaxTrees = syntaxTrees;
            ModuleLookup = moduleLookup;
            ModuleFailureLookup = moduleFailureLookup;
        }
    }
}