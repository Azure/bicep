// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Navigation;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;

namespace Bicep.LanguageServer.Completions
{
    public class BicepCompletionContext
    {
        public BicepCompletionContext(BicepCompletionContextKind kind)
        {
            this.Kind = kind;
        }

        public BicepCompletionContextKind Kind { get; }

        public static BicepCompletionContext Create(ProgramSyntax syntax, int offset)
        {
            var node = syntax.TryFindMostSpecificNodeInclusive(offset, current => !(current is Token));
            if (node == null)
            {
                return new BicepCompletionContext(BicepCompletionContextKind.None);
            }

            var kind = IsDeclarationContext(syntax, offset, node) ? BicepCompletionContextKind.Declaration : BicepCompletionContextKind.None;

            return new BicepCompletionContext(kind);
        }

        private static bool IsDeclarationContext(ProgramSyntax syntax, int offset, SyntaxBase mostSpecificNode) => mostSpecificNode is ProgramSyntax;
    }
}
