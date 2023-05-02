// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.Core.PrettyPrintV2.Documents
{
    public static class DocumentOperators
    {
        public static readonly Document LiteralLine = new LineDocument(null);

        public static readonly Document LineOrEmpty = new LineDocument("");

        public static readonly Document LineOrSpace = new LineDocument(" ");

        public static readonly Document LineOrCommaSpace = new LineDocument(", ");

        public static readonly Document CommaLineOrCommaSpace = Concat(",", LineOrSpace);

        public static IEnumerable<TextDocument> Flatten(this IEnumerable<Document> documents) => documents.SelectMany(x => x.Flatten());

        public static ConcatDocument Concat(Document first, Document second, params Document[] tail) => Concat(first.Append(second).Concat(tail));

        public static ConcatDocument Concat(IEnumerable<Document> documents) => new(documents);

        public static ConcatDocument SpaceSeparated(Document first, Document second, params Document[] tail) => SeparateWithSpace(first.Append(second).Concat(tail));

        public static ConcatDocument SeparateWithSpace(IEnumerable<Document> documents) => Concat(documents.SeparatedBy(" "));

        public static IndentDocument Indent(Document first, Document second, params Document[] tail) => Indent(tail.Prepend(second).Prepend(first));

        public static IndentDocument Indent(IEnumerable<Document> documents) => new(documents);

        public static GroupDocument Group(Document first, Document second, params Document[] tail) => Group(tail.Prepend(second).Prepend(first));

        public static GroupDocument Group(IEnumerable<Document> documents) => new(documents);

        public static IEnumerable<Document> SeparatedBy(this IEnumerable<Document> documents, Document seperator)
        {
            using var enumerator = documents.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                yield break;
            }

            var first = enumerator.Current;

            if (!enumerator.MoveNext())
            {
                // Only one document available.
                yield return first;
            }
            else
            {
                do
                {
                    yield return seperator;
                    yield return enumerator.Current;
                }
                while (enumerator.MoveNext());
            }
        }

        public static IEnumerable<Document> Collapse(this IEnumerable<Document> documents, Predicate<Document> predicate)
        {
            var documentList = new List<Document>();

            foreach (var document in documents)
            {
                if (documentList.Count == 0 || !predicate(document) || document != documentList[^1])
                {
                    documentList.Add(document);
                }
            }

            return documentList;
        }

        public static IEnumerable<Document> Trim(this IEnumerable<Document> documents, Predicate<Document> predicate)
        {
            if (!documents.Any())
            {
                return documents;
            }

            var documentArray = documents.ToArray();

            var start = 0;
            var end = documentArray.Length - 1;

            while (predicate(documentArray[start]))
            {
                start++;
            }

            while (predicate(documentArray[end]))
            {
                end--;
            }

            if (start > end)
            {
                return Enumerable.Empty<Document>();
            }

            return documentArray[start..(end + 1)];
        }
    }
}
