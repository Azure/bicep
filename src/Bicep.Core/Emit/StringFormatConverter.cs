// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Text;
using Bicep.Core.Syntax;

namespace Bicep.Core.Emit
{
    public static class StringFormatConverter
    {
        public static string BuildFormatString(ImmutableArray<string> segmentValues)
        {
            var stringBuilder = new StringBuilder();

            for (var i = 0; i < segmentValues.Length - 1; i++)
            {
                AppendAndEscapeCurlies(stringBuilder, segmentValues[i]);
                stringBuilder.Append('{');
                stringBuilder.Append(i);
                stringBuilder.Append('}');
            }

            AppendAndEscapeCurlies(stringBuilder, segmentValues[segmentValues.Length - 1]);

            return stringBuilder.ToString();
        }

        private static void AppendAndEscapeCurlies(StringBuilder buffer, string input)
        {
            // ARM uses the string.Format() function which expects curlies to be esaped
            for (var i = 0; i < input.Length; i++)
            {
                switch (input[i])
                {
                    case '{':
                        buffer.Append("{{");
                        break;
                    case '}':
                        buffer.Append("}}");
                        break;
                    default:
                        buffer.Append(input[i]);
                        break;
                }
            }
        }
    }
}

