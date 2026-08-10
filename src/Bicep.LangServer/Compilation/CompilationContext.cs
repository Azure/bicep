// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using BicepCompilation = Bicep.Core.Semantics.Compilation;

namespace Bicep.LanguageServer.Compilation
{
    /// <summary>
    /// Represents a compilation context that successfully produced a compilation
    /// (the compilation itself may have errors or warnings in the semantic model)
    /// </summary>
    public class CompilationContext : CompilationContextBase
    {
        public CompilationContext(BicepCompilation compilation)
            // on a successful compilation, we can reuse the entry point file kind
            : base(compilation.SourceFileGrouping.EntryPoint.FileKind)
        {
            this.Compilation = compilation;
        }

        public BicepCompilation Compilation { get; }

        public ProgramSyntax ProgramSyntax => Compilation.SourceFileGrouping.EntryPoint.ProgramSyntax;

        public ImmutableArray<int> LineStarts => Compilation.SourceFileGrouping.EntryPoint.LineStarts;
    }
}
