using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bicep.Core.Parser;
using Newtonsoft.Json;

namespace Bicep.Core.Syntax
{
    public static class StringSyntaxExtensions
    {
        public static string? TryGetFormatString(this StringSyntax syntax)
        {
            var stringBuilder = new StringBuilder();
            var segments = new List<string>();

            foreach (var segment in syntax.StringTokens.Select(Lexer.TryGetStringValue))
            {
                if (segment == null)
                {
                    return null;
                }

                segments.Add(segment);
            }

            return BuildFormatString(segments);
        }

        public static string GetFormatString(this StringSyntax syntax)
        {
            var segments = syntax.StringTokens.Select(Lexer.GetStringValue);

            return BuildFormatString(segments);
        }

        private static string BuildFormatString(IEnumerable<string> segments)
        {
            var stringBuilder = new StringBuilder();
            var segmentsArray = segments.ToArray();

            for (var i = 0; i < segmentsArray.Length - 1; i++)
            {
                stringBuilder.Append(segmentsArray[i]);
                stringBuilder.Append('{');
                stringBuilder.Append(i);
                stringBuilder.Append('}');
            }

            stringBuilder.Append(segmentsArray[segmentsArray.Length - 1]);

            return stringBuilder.ToString();
        }
    }
}
