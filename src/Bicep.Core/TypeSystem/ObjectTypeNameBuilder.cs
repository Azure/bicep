// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Text;
using Bicep.Core.Parsing;

namespace Bicep.Core.TypeSystem;

[DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
internal class ObjectTypeNameBuilder
{
    private readonly StringBuilder builder = new("{");
    private bool hasProperties = false;
    private bool finalized = false;

    internal void AppendProperty(string propertyName, string propertyValue)
        => DoAppendProperty(StringUtils.EscapeBicepPropertyName(propertyName), propertyValue);

    internal void AppendPropertyMatcher(string matchNotation, string value)
        => DoAppendProperty(matchNotation, value);

    internal void AppendPropertyMatcher(string value)
        => AppendPropertyMatcher("*", value);

    private void DoAppendProperty(string propertyName, string propertyValue)
    {
        if (finalized)
        {
            throw new InvalidOperationException("AppendProperty may not be called after the object type name has been finalized.");
        }

        if (hasProperties)
        {
            builder.Append(',');
        }
        hasProperties = true;
        builder.Append(' ');
        builder.Append(propertyName);

        builder.Append(": ");
        builder.Append(propertyValue);
    }

    public override string ToString()
    {
        if (!finalized)
        {
            builder.Append(" }");
            finalized = true;
        }

        return builder.ToString();
    }

    // use the debugger display to avoid calling ToString (which modifies state) in the debugger
    // this avoids the InvalidOperationException thrown in DoAppendProperty
    private string GetDebuggerDisplay() => builder.ToString();
}
