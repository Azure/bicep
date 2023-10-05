// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Deployments.Core.Extensions;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.PrettyPrintV2.Documents;
using Bicep.Core.Syntax;

namespace Bicep.Core.PrettyPrintV2
{
    public class PrettyPrinterV2
    {
        private readonly NoTrailingSpaceWriter writer;

        private readonly PrettyPrinterV2Context context;

        private int occupiedWidth = 0;

        private PrettyPrinterV2(TextWriter writer, PrettyPrinterV2Context context)
        {
            this.writer = new(writer, context);
            this.context = context;
        }

        public static string Print(PrettyPrinterV2Context context)
        {
            var writer = new StringWriter();

            PrintTo(writer, context);

            return writer.ToString();
        }

        public static void PrintTo(TextWriter writer, PrettyPrinterV2Context context)
        {
            var layouts = new SyntaxLayouts(context);
            var documents = layouts.Layout(context.SyntaxToPrint);
            var printer = new PrettyPrinterV2(writer, context);

            foreach (var document in documents)
            {
                switch (document)
                {
                    case IndentDocument indentDocument:
                        printer.Print(1, indentDocument.Documents);
                        break;

                    case GlueDocument glueDocument:
                        printer.Print(0, glueDocument.Documents);
                        break;

                    default:
                        printer.Print(0, document);
                        break;
                }
            }

            if (context.Options.InsertFinalNewline)
            {
                writer.Write(context.Newline);
            }
        }

        private static IEnumerable<(int indentLevel, Document document)> Normalize(int indentLevel, ImmutableArray<Document> documents)
        {
            var enumeratorStack = new Stack<ImmutableArray<Document>.Enumerator>();
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

                        case GlueDocument glueDocument:
                            enumeratorStack.Push(enumerator);
                            enumerator = glueDocument.Documents.GetEnumerator();
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

                enumerator = enumeratorStack.Pop();

                if (enumerator.Current is IndentDocument)
                {
                    indentLevel--;
                }
            }
        }

        private void Print(int indentLevel, ImmutableArray<Document> documents)
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
                    this.writer.Write(text);
                    this.occupiedWidth += text.Width;

                    return;


                case LineDocument:
                    this.writer.WriteLine();
                    this.writer.WriteIndent(indentLevel);
                    this.occupiedWidth = this.context.Indent.Length * indentLevel;

                    return;

                case GroupDocument group:
                    var remainingWidth = this.context.Width - this.occupiedWidth;

                    if (remainingWidth < 0)
                    {
                        Print(indentLevel, group.Documents);

                        return;
                    }

                    var flattened = ImmutableArray.CreateBuilder<Document>();

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

                    Print(indentLevel, flattened.ToImmutable());

                    return;


                default:
                    throw new NotImplementedException();
            }
        }

        private class NoTrailingSpaceWriter
        {
            private readonly TextWriter writer;

            private readonly PrettyPrinterV2Context context;

            private readonly StringBuilder spaceBuffer;

            public NoTrailingSpaceWriter(TextWriter writer, PrettyPrinterV2Context context)
            {
                this.writer = writer;
                this.context = context;
                this.spaceBuffer = new();
            }

            public void Write(string text)
            {
                if (string.IsNullOrEmpty(text))
                {
                    return;
                }

                if (text.All(char.IsWhiteSpace))
                {
                    spaceBuffer.Append(text);

                    return;
                }

                if (spaceBuffer.Length > 0)
                {
                    this.writer.Write(spaceBuffer.ToString());
                    spaceBuffer.Clear();
                }

                this.writer.Write(text);
            }

            public void WriteLine()
            {
                this.spaceBuffer.Clear();
                this.writer.Write(this.context.Newline);
            }

            public void WriteIndent(int indentLevel)
            {
                for (int i = 0; i < indentLevel; i++)
                {
                    this.spaceBuffer.Append(this.context.Indent);
                }
            }
        }
    }
}
