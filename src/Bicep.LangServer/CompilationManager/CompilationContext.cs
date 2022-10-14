// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System.Collections.Immutable;

namespace Bicep.LanguageServer.CompilationManager
{
    /// <summary>
    /// Represents a compilation context that successfully produced a compilation
    /// (the compilation itself may have errors or warnings in the semantic model)
    /// </summary>
    public class CompilationContext : CompilationContextBase
    {
        public CompilationContext(Compilation compilation)
            // on a successful compilation, we can reuse the entry point file kind
            : base(compilation.SourceFileGrouping.EntryPoint.FileKind)
        {
            this.Compilation = compilation;
        }

        public Compilation Compilation { get; }

        public ProgramSyntax ProgramSyntax => Compilation.SourceFileGrouping.EntryPoint.ProgramSyntax;

        public ImmutableArray<int> LineStarts => Compilation.SourceFileGrouping.EntryPoint.LineStarts;
    }
}
