// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax
{
    public class SyntaxTree
    {
        public static SyntaxTree Create(string filePath, string fileContents)
        {
            var parser = new Parser.Parser(fileContents);
            var lineStarts = TextCoordinateConverter.GetLineStarts(fileContents);
            
            return new SyntaxTree(filePath, lineStarts, parser.Program());
        }

        public SyntaxTree(string filePath, ImmutableArray<int> lineStarts, ProgramSyntax programSyntax)
        {
            FilePath = filePath;
            LineStarts = lineStarts;
            ProgramSyntax = programSyntax;
        }

        public string FilePath { get; }

        public ImmutableArray<int> LineStarts { get; }

        public ProgramSyntax ProgramSyntax { get; }
    }
}