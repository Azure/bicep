// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Text;

namespace Bicep.Core.Extensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder TrimNewLines(this StringBuilder sb)
        {
            if (sb.Length == 0)
            {
                return sb;
            }

            // trim end.
            int position = sb.Length - 1;

            while (position >= 0)
            {
                if (!IsNewlineCharacter(sb[position]))
                {
                    break;
                }

                position--;
            }

            if (position < sb.Length - 1)
            {
                sb.Length = position + 1;
            }

            // trim start.
            position = 0;

            while (position < sb.Length)
            {
                if (!IsNewlineCharacter(sb[position]))
                {
                    break;
                }

                position++;
            }

            if (position > 0)
            {
                sb.Remove(0, position);
            }

            return sb;
        }

        private static bool IsNewlineCharacter(char c)
        {
            return c == '\r' || c == '\n';
        }
    }
}
