// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Core.Extensions;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.PrettyPrintV2.Documents;
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
    public class PrettyPrinterV2
    {
        private readonly TextWriter writer;

        private readonly PrettyPrinterV2Context context;

        private int occupiedWidth = 0;

        private int indentLevelToPrint = 0;

        public PrettyPrinterV2(TextWriter writer, PrettyPrinterV2Context context)
        {
            this.writer = writer;
            this.context = context;
        }

        public static void PrintTo(TextWriter writer, SyntaxBase syntax, PrettyPrinterV2Options options, IDiagnosticLookup lexingErrorLookup, IDiagnosticLookup parsingErrorLookup)
        {
            var context = PrettyPrinterV2Context.Create(syntax, options, lexingErrorLookup, parsingErrorLookup);
            var layouts = new SyntaxLayouts(context);
            var documents = layouts.Layout(syntax);
            var printer = new PrettyPrinterV2(writer, context);

            printer.Print(0, documents);

            if (options.InsertFinalNewline)
            {
                writer.Write(context.Newline);
            }
        }

        private static IEnumerable<(int indentLevel, Document document)> Normalize(int indentLevel, IEnumerable<Document> documents)
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
                            enumerator = indentDocument.Documents.AsEnumerable().GetEnumerator();
                            indentLevel++;
                            break;

                        case GlueDocument concatDocument:
                            enumeratorStack.Push(enumerator);
                            enumerator = concatDocument.Documents.AsEnumerable().GetEnumerator();
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

        private void Print(int indentLevel, IEnumerable<Document> documents)
        {
            foreach (var (normalizedIndentLevel, document) in Normalize(indentLevel, documents))
            {
                this.Print(normalizedIndentLevel, document);
            }
        }

        private void Print(int indentLevel, Document document)
        {
            switch (document)
            {
                case TextDocument text:
                    this.PrintIndent();

                    this.writer.Write(text);
                    this.occupiedWidth += text.Width;

                    return;


                case LineDocument:
                    this.writer.Write(this.context.Newline);

                    this.occupiedWidth = 0;
                    this.indentLevelToPrint = indentLevel;

                    return;

                case GroupDocument group:
                    this.PrintIndent();

                    var remainingWidth = this.context.Width - this.occupiedWidth;

                    if (remainingWidth < 0)
                    {
                        Print(indentLevel, group.Documents);

                        return;
                    }

                    var flattened = new List<Document>();

                    foreach (var child in group.Flatten())
                    {
                        remainingWidth -= child.Width;

                        if (remainingWidth < 0)
                        {
                            // Short circuiting to avoid flattening the remaning children.
                            Print(indentLevel, group.Documents);

                            return;
                        }

                        flattened.Add(child);
                    }

                    Print(indentLevel, flattened);

                    return;


                default:
                    throw new NotImplementedException();
            }
        }

        private void PrintIndent()
        {
            if (this.indentLevelToPrint == 0)
            {
                return;
            }

            for (int i = 0; i < this.indentLevelToPrint; i++)
            {
                this.writer.Write(this.context.Indent);
                this.occupiedWidth += this.context.Indent.Length;
            }

            this.indentLevelToPrint = 0;
        }
    }
}
