// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using Bicep.Core.Syntax;

namespace Bicep.Core.Workspaces
{
    public class BicepFile : ISourceFile
    {
        public BicepFile(Uri fileUri, ImmutableArray<int> lineStarts, ProgramSyntax programSyntax)
        {
            FileUri = fileUri;
            LineStarts = lineStarts;
            ProgramSyntax = programSyntax;
            Hierarchy = new SyntaxHierarchy();
            Hierarchy.AddRoot(ProgramSyntax);
        }

        public BicepFile(BicepFile original)
        {
            FileUri = original.FileUri;
            LineStarts = original.LineStarts;
            ProgramSyntax = original.ProgramSyntax;
            Hierarchy = original.Hierarchy;
        }

        public Uri FileUri { get; }

        public ImmutableArray<int> LineStarts { get; }

        public ProgramSyntax ProgramSyntax { get; }

        public SyntaxHierarchy Hierarchy { get; }
    }
}
