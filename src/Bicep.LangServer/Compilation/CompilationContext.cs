// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.LanguageServer.Compilation
{
    /// <summary>
    /// Represents a compilation context that successfully produced a compilation
    /// (the compilation itself may have errors or warnings in the semantic model)
    /// </summary>
    public class CompilationContext : CompilationContextBase
    {
        public CompilationContext(global::Bicep.Core.Semantics.Compilation compilation)
            // on a successful compilation, we can reuse the entry point file kind
            : base(compilation.SourceFileGrouping.EntryPoint.FileKind)
        {
            this.Compilation = compilation;
        }

        public global::Bicep.Core.Semantics.Compilation Compilation { get; }

        public ProgramSyntax ProgramSyntax => Compilation.SourceFileGrouping.EntryPoint.ProgramSyntax;

        public ImmutableArray<int> LineStarts => Compilation.SourceFileGrouping.EntryPoint.LineStarts;
    }
}
