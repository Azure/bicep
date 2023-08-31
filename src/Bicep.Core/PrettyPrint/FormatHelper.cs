// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Navigation;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using System;

namespace Bicep.Core.PrettyPrint
{
    public static class FormatHelper
    {
        /// <summary>
        /// Calculates the number of spaces that a particular piece of syntax is indented by.
        /// </summary>
        /// <param name="semanticModel">The semantic model.</param>
        /// <param name="syntax">The piece of syntax to calculate indenting for.</param>
        public static int GetIndentLevel(SemanticModel semanticModel, SyntaxBase syntax)
        {
            var ancestors = semanticModel.Binder.GetAllAncestors<SyntaxBase>(syntax);

            var indentLevel = 0;
            for (var i = ancestors.Length - 1; i >= 0; i--)
            {
                if (ancestors[i] is ObjectPropertySyntax objectProperty &&
                    i > 0 && ancestors[i - 1] is ObjectSyntax @object)
                {
                    i--;

                    // Try and search upwards to find the first place where the object and property are on different lines
                    var propPosition = TextCoordinateConverter.GetPosition(semanticModel.SourceFile.LineStarts, objectProperty.Span.Position);
                    var objPosition = TextCoordinateConverter.GetPosition(semanticModel.SourceFile.LineStarts, @object.Span.Position);

                    if (propPosition.line != objPosition.line)
                    {
                        indentLevel = propPosition.character;
                        break;
                    }

                    continue;
                }

                if (ancestors[i] is ArrayItemSyntax arrayItem &&
                    i > 0 && ancestors[i - 1] is ArraySyntax array)
                {
                    i--;

                    // Try and search upwards to find the first place where the array and property are on different lines
                    var propPosition = TextCoordinateConverter.GetPosition(semanticModel.SourceFile.LineStarts, arrayItem.Span.Position);
                    var objPosition = TextCoordinateConverter.GetPosition(semanticModel.SourceFile.LineStarts, array.Span.Position);

                    if (propPosition.line != objPosition.line)
                    {
                        indentLevel = propPosition.character;
                        break;
                    }

                    continue;
                }
            }

            return indentLevel;
        }

        /// <summary>
        /// Prints a new piece of syntax to string, using the position of an existing piece of syntax on the syntax tree to correctly calculate indenting.
        /// Useful if you intend to replace the old syntax with the new syntax (e.g. in a code action) and have the indenting fit correctly with the document.
        /// </summary>
        /// <param name="semanticModel">The semantic model.</param>
        /// <param name="oldSyntax">The piece of existing syntax to use for indenting calculations.</param>
        /// <param name="newSyntax">The piece syntax to print with correct indenting.</param>
        /// <param name="tabSize">The assumed tab size of the document (number of spaces for each level of indenting).</param>
        public static string ToFormattedTextWithNewIndentation(SemanticModel semanticModel, SyntaxBase oldSyntax, SyntaxBase newSyntax, int tabSize)
        {
            var indentLevel = GetIndentLevel(semanticModel, oldSyntax);

            return newSyntax.ToText(
                indent: new String(' ', tabSize),
                newLineSequence: $"{Environment.NewLine}{new String(' ', indentLevel)}");
        }
    }
}
