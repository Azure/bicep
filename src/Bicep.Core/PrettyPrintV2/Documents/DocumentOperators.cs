// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Core.Extensions;
using Azure.ResourceManager.Resources.Models;
using JetBrains.Annotations;
using Microsoft.WindowsAzure.ResourceStack.Common.Algorithms;
using Microsoft.WindowsAzure.ResourceStack.Common.Storage;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.Core.PrettyPrintV2.Documents
{
    public static class DocumentOperators
    {
        public static readonly IEnumerable<Document> Empty = Enumerable.Empty<Document>();

        /// <summary>
        /// Prints a whitespace.
        /// </summary>
        public static readonly TextDocument Space = TextDocument.From(" ");

        /// <summary>
        /// Prints a newline that is always included in the output and doesn't indent the next line.
        /// </summary>
        public static readonly LineDocument HardLine = new(null);

        /// <summary>
        /// A newline placeholder that may or may not be printed.
        /// </summary>
        public static readonly LineDocument SoftLine = new(null);

        /// <summary>
        /// Prints a newline and indent the next line. If the enclosing group fits on one line, the newline will be replaced with an empty string.
        /// </summary>
        public static readonly LineDocument LineOrEmpty = new("");

        /// <summary>
        /// Prints a newline and indent the next line. If the enclosing group fits on one line, the newline will be replaced with a whitespace.
        /// </summary>
        public static readonly LineDocument LineOrSpace = new(Space);

        /// <summary>
        /// Prints a newline and indent the next line. If the enclosing group fits on one line, the newline will be replaced with a comma followed by a whitespace.
        /// </summary>
        public static readonly LineDocument LineOrCommaSpace = new(", ");

        /// <summary>
        /// Prints a comma and newline and indent the next line. If the enclosing group fits on one line, the newline will be replaced with a whitespace.
        /// </summary>
        public static readonly Document CommaLineOrCommaSpace = Glue(",", LineOrSpace);

        public static Document Glue(params Document[] documents) => new GlueDocument(documents);

        public static Document Glue(this IEnumerable<Document> documents) => documents is Document single ? single : new GlueDocument(documents);

        public static Document Spread(this IEnumerable<Document> documents) => documents.SeparateBySpace().Glue();

        public static IndentDocument Indent(this IEnumerable<Document> documents) => new(documents);

        public static GroupDocument Group(params Document[] documents) => new(documents);

        public static IEnumerable<Document> SeparateBy(this IEnumerable<Document> documents, Document separator)
        {
            using var enumerator = documents.GetEnumerator();

            if (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }

            var previousIsLiteralLine = enumerator.Current == HardLine;

            while (enumerator.MoveNext())
            {
                if (!previousIsLiteralLine)
                {
                    yield return separator;
                }

                yield return enumerator.Current;
                previousIsLiteralLine = enumerator.Current == HardLine;
            }
        }

        public static IEnumerable<Document> SeparateBySpace(this IEnumerable<Document> documents) => documents.SeparateBy(Space);

        public static IEnumerable<Document> SeparatedByNewline(this IEnumerable<Document> documents) => documents.SeparateBy(HardLine);

        public static IEnumerable<Document> TrimNewlines(this IEnumerable<Document> documents)
        {
            if (!documents.Any())
            {
                return documents;
            }

            var documentArray = documents.ToArray();

            var start = 0;
            var end = documentArray.Length - 1;

            while (start <= end && documentArray[start] is LineDocument)
            {
                start++;
            }

            if (start > end)
            {
                return Empty;
            }

            while (documentArray[end] is LineDocument)
            {
                end--;
            }

            if (start > end)
            {
                return Empty;
            }

            return documentArray[start..(end + 1)];
        }

        public static IEnumerable<Document> CollapseNewlines(this IEnumerable<Document> documents, Action? onHardLine = null)
        {
            using var enumerator = documents.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                yield break;
            }

            var current = enumerator.Current;

            if (current != SoftLine)
            {
                yield return current;

                if (current == HardLine)
                {
                    onHardLine?.Invoke();
                }
            }

            var previous = current;

            while (enumerator.MoveNext())
            {
                current = enumerator.Current;

                if (current == SoftLine)
                {
                    if (previous == SoftLine)
                    {
                        // Collapse consecutive SoftLines into a HardLine.
                        yield return HardLine;

                        onHardLine?.Invoke();
                        previous = HardLine;
                    }

                    previous = SoftLine;
                }
                else if (current == HardLine)
                {
                    if (previous != HardLine)
                    {
                        yield return HardLine;

                        onHardLine?.Invoke();
                    }

                    previous = HardLine;
                }
                else
                {
                    yield return current;

                    previous = current;
                }
            }
        }
    }
}
