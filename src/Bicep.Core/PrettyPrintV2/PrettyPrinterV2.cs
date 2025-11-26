// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text;
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

        /// <summary>
        /// Outputs a formatted string representation of a syntactically correct syntax node.
        /// </summary>
        /// <remarks>
        /// This method is intended for formatting manually created syntax nodes that are confirmed to be free of lexing and parsing errors.
        /// Supplying a syntax node that contains errors could result in improperly formatted output.
        /// </remarks>
        /// <param name="validSyntaxToPrint">The syntax node to format, which must be free of syntax errors.</param>
        /// <param name="options">The formatting options.</param>
        /// <returns>A string that represents the formatted syntax node.</returns>
        public static string PrintValid(SyntaxBase validSyntaxToPrint, PrettyPrinterV2Options options)
        {
            var context = PrettyPrinterV2Context.Create(options, EmptyDiagnosticLookup.Instance, EmptyDiagnosticLookup.Instance);

            return Print(validSyntaxToPrint, context);
        }

        public static string Print(SyntaxBase syntaxToPrint, PrettyPrinterV2Context context)
        {
            var writer = new StringWriter();

            PrintTo(writer, syntaxToPrint, context);

            return writer.ToString();
        }

        public static void PrintTo(TextWriter writer, SyntaxBase syntaxToPrint, PrettyPrinterV2Context context)
        {
            var layouts = new SyntaxLayouts(context);
            var documents = layouts.Layout(syntaxToPrint);
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

            if (syntaxToPrint is ProgramSyntax && context.Options.InsertFinalNewline)
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
                    var forceFlatten = false;

                    foreach (var child in group.Flatten())
                    {
                        if (forceFlatten)
                        {
                            flattened.Add(child);
                            continue;
                        }

                        remainingWidth -= child.Width;

                        if (remainingWidth < 0)
                        {
                            // Short circuiting to avoid flattening the remaining children.
                            Print(indentLevel, group.Documents);

                            return;
                        }

                        if (child is FunctionTailObjectOrArrayDocument)
                        {
                            // It is safe to flatten everything after FunctionTailObjectOrArrayDocument, since
                            // there is no LineDocuments after it to be flattened to alternative layouts.
                            forceFlatten = true;
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
