// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bicep.RegistryModuleTool.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsEmpty<T>(this IEnumerable<T> enumerable) => !enumerable.Any();

        public static string ToMarkdownTable<T>(this IEnumerable<T> enumerable, Func<string, MarkdownTableColumnAlignment> columnAlignmentSelector) where T : class
        {
            var type = typeof(T);
            var properties = typeof(T).GetProperties();

            if (properties.Length == 0)
            {
                throw new ArgumentException($"Expected {type.Name} to have at least one property.");
            }

            var columnNames = properties.Select(x => x.Name).ToArray();
            var maximumColumnWidths = enumerable
                .Select(item => properties.Select(property => property.GetValue(item)?.ToString()?.Length ?? 0))
                .Union(new[] { properties.Select(property => property.Name.Length) })
                .Aggregate(new int[properties.Length].AsEnumerable(), (maximumWidths, widths) => maximumWidths.Zip(widths, Math.Max))
                .ToArray();

            var builder = new StringBuilder();

            // Build header line.
            for (int i = 0; i < columnNames.Length; i++)
            {
                builder.Append("| ");
                builder.Append(columnNames[i].PadRight(maximumColumnWidths[i]));
                builder.Append(' ');
            }

            builder.AppendLine("|");

            // Build divider line.
            for (int i = 0; i < columnNames.Length; i++)
            {
                var alignment = columnAlignmentSelector(columnNames[i]);

                builder.Append("| ");
                builder.Append(alignment != MarkdownTableColumnAlignment.Right ? ":" : "-");
                builder.Append(new string('-', Math.Max(0, maximumColumnWidths[i] - 2)));
                builder.Append(alignment != MarkdownTableColumnAlignment.Left ? ":" : "-");
                builder.Append(' ');
            }

            builder.AppendLine("|");

            // Build data lines.
            foreach (var item in enumerable)
            {
                for (int i = 0; i < columnNames.Length; i++)
                {
                    builder.Append("| ");
                    builder.Append((properties[i].GetValue(item)?.ToString() ?? "").PadRight(maximumColumnWidths[i]));
                    builder.Append(' ');
                }

                builder.AppendLine("|");
            }

            return builder.ToString();
        }
    }
}
