// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Core.Extensions;
using Bicep.Core.PrettyPrintV2.Documents;
using Bicep.Core.PrettyPrintV2.Options;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.PrettyPrintV2
{
    public class PrettyPrinter
    {
        private readonly PrettyPrintOptions options;

        private readonly IReadOnlySet<GroupDocument> lineBreakingGroups;

        private readonly TextWriter writer;

        public PrettyPrinter(PrettyPrintOptions options, TextWriter writer, IReadOnlySet<GroupDocument> lineBreakingGroups)
        {
            this.options = options;
            this.writer = writer;
            this.lineBreakingGroups = lineBreakingGroups;
        }


        public static void Print(SyntaxBase syntax, PrettyPrintOptions options, TextWriter writer)
        {
            var lineBreakingGroups = new HashSet<GroupDocument>();
            var layouts = new SyntaxLayouts(lineBreakingGroups);
            var documents = layouts.Layout(syntax);
            var printer = new PrettyPrinter(options, writer, lineBreakingGroups);

            printer.Print(documents);
        }

        private void Print(IEnumerable<Document> documents)
        {
            var occupied = 0;

            this.Print(0, documents, ref occupied);
        }

        private void Print(int indentLevel, IEnumerable<Document> documents, ref int occupied)
        {
            foreach (var (currentIndentLevel, document) in this.Normalize(indentLevel, documents))
            {
                this.Print(currentIndentLevel, document, ref occupied);
            }
        }

        private void Print(int indentLevel, Document document, ref int occupied)
        {
            switch (document)
            {
                case TextDocument text:
                    this.writer.Write(text);
                    occupied += text.Width;
                    return;

                case LineDocument:
                    this.writer.WriteLine();
                    this.writer.Write(new string(' ', indentLevel));
                    occupied = indentLevel;
                    return;

                case GroupDocument group:
                    var remaining = this.options.Width - occupied;

                    if (remaining < 0)
                    {
                        Print(indentLevel, group.Documents, ref occupied);
                    }
                    else
                    {
                        var flattened = group.Flatten().ToList();
                        var flattenedWidth = flattened.Sum(x => x.Width);
                        var printable = flattenedWidth <= remaining ? flattened : group.Documents;

                        Print(indentLevel, printable, ref occupied);
                    }

                    return;

                default:
                    throw new InvalidOperationException("Not printable.");
            }
        }

        private IEnumerable<(int indentLevel, Document document)> Normalize(int indentLevel, IEnumerable<Document> documents)
        {
            var enumeratorStack = new Stack<IEnumerator<Document>>();
            var enumerator = documents.GetEnumerator();

            while (true)
            {
                while (enumerator.MoveNext())
                {
                    var document = enumerator.Current;

                    switch (enumerator.Current)
                    {
                        case IndentDocument indentDocument:
                            enumeratorStack.Push(enumerator);
                            enumerator = indentDocument.Documents.GetEnumerator();
                            indentLevel++;
                            break;

                        case ConcatDocument concatDocument:
                            enumeratorStack.Push(enumerator);
                            enumerator = concatDocument.Documents.GetEnumerator();
                            break;

                        case GroupDocument groupDocument when this.lineBreakingGroups.Contains(groupDocument):
                            enumeratorStack.Push(enumerator);
                            enumerator = groupDocument.Documents.GetEnumerator();
                            break;

                        default:
                            yield return (indentLevel, document);
                            break;
                    }
                }

                if (enumeratorStack.Count == 0)
                {
                    break;
                }

                enumerator.Dispose();
                enumerator = enumeratorStack.Pop();

                if (enumerator.Current is IndentDocument)
                {
                    indentLevel--;
                }
            }
        }
    }
}
