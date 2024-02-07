// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.LanguageServer.CompilationManager
{
    /// <summary>
    /// Represents a compilation context that successfully produced a compilation
    /// (the compilation itself may have errors or warnings in the semantic model)
    /// </summary>
    public class CompilationContext(Compilation compilation) : CompilationContextBase(compilation.SourceFileGrouping.EntryPoint.FileKind)
    {
        public Compilation Compilation { get; } = compilation;

        public ProgramSyntax ProgramSyntax => Compilation.SourceFileGrouping.EntryPoint.ProgramSyntax;

        public ImmutableArray<int> LineStarts => Compilation.SourceFileGrouping.EntryPoint.LineStarts;
    }
}
