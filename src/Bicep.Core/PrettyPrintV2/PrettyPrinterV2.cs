// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Core.Extensions;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
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
    public class PrettyPrinterV2
    {
        private readonly TextWriter writer;

        private readonly PrettyPrintOptionsV2 options;

        private int occupied = 0;

        private int indentLevelToPrint = 0;

        public PrettyPrinterV2(TextWriter writer, PrettyPrintOptionsV2 options)
        {
            this.writer = writer;
            this.options = options;
        }

        public void Print(SyntaxBase syntax, IDiagnosticLookup lexingErrorLookup, IDiagnosticLookup parsingErrorLookup)
        {
            var layouts = new SyntaxLayouts(this.options, lexingErrorLookup, parsingErrorLookup);
            var documents = layouts.Layout(syntax).ToImmutableArray();

            this.Print(0, documents);
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

                        case GlueDocument concatDocument:
                            enumeratorStack.Push(enumerator);
                            enumerator = concatDocument.Documents.GetEnumerator();
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
                    this.PrintIndent();

                    this.writer.Write(text);
                    this.occupied += text.Width;

                    return;


                case LineDocument:
                    this.writer.Write(this.options.Newline);

                    this.occupied = indentLevel * this.options.Indent.Length;
                    this.indentLevelToPrint = indentLevel;

                    return;

                case GroupDocument group:
                    var remaining = this.options.Width - this.occupied;

                    if (remaining < 0)
                    {
                        Print(indentLevel, group.Documents);

                        return;
                    }

                    var flattened = ImmutableArray.CreateBuilder<Document>();

                    foreach (var child in group.Flatten())
                    {
                        remaining -= child.Width;

                        if (remaining < 0)
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

        private void PrintIndent()
        {
            if (this.indentLevelToPrint == 0)
            {
                return;
            }

            for (int i = 0; i < this.indentLevelToPrint; i++)
            {
                this.writer.Write(this.options.Indent);
            }

            this.indentLevelToPrint = 0;
        }
    }
}
