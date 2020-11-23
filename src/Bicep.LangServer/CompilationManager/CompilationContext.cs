// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.LanguageServer.CompilationManager
{
    public class CompilationContext
    {
        public CompilationContext(Compilation compilation)
        {
            this.Compilation = compilation;
        }

        public Compilation Compilation { get; }

        public ProgramSyntax ProgramSyntax => Compilation.SyntaxTreeGrouping.EntryPoint.ProgramSyntax;

        public ImmutableArray<int> LineStarts => Compilation.SyntaxTreeGrouping.EntryPoint.LineStarts;
    }
}
