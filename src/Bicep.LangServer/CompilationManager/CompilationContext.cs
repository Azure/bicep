// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Parser;
using Bicep.Core.SemanticModel;

namespace Bicep.LanguageServer.CompilationManager
{
    public class CompilationContext
    {
        public CompilationContext(Compilation compilation, ImmutableArray<int> lineStarts)
        {
            this.Compilation = compilation;
            this.LineStarts = lineStarts;
        }

        public Compilation Compilation { get; }

        public ImmutableArray<int> LineStarts { get; }
    }
}
