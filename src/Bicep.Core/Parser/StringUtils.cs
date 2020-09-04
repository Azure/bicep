// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Text;

namespace Bicep.Core.Parser
{
    public static class StringUtils
    {
        public static string EscapeBicepString(string value)
        {
            var buffer = new StringBuilder(value.Length + 2);

            buffer.Append('\'');
            for (var i = 0; i < value.Length; i++)
            {
                switch (value[i])
                {
                    case '\\':
                        buffer.Append("\\\\");
                        break;
                    case '\'':
                        buffer.Append("\\'");
                        break;
                    case '\r':
                        buffer.Append("\\r");
                        break;
                    case '\n':
                        buffer.Append("\\n");
                        break;
                    case '\t':
                        buffer.Append("\\t");
                        break;
                    case '$' when value.Length > i + 1 && value[i + 1] == '{':
                        buffer.Append("\\$");
                        break;
                    default:
                        buffer.Append(value[i]);
                        break;
                }
            }
            buffer.Append('\'');

            return buffer.ToString();
        }
    }
}