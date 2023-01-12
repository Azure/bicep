// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Text;
using Bicep.Core.Parsing;

namespace Bicep.Core.TypeSystem;

internal class ObjectTypeNameBuilder
{
    private readonly StringBuilder builder = new("{");
    private bool hasProperties = false;
    private bool finalized = false;

    internal void AppendProperty(string propertyName, string propertyValue)
        => DoAppendProperty(Lexer.IsValidIdentifier(propertyName) ? propertyName : StringUtils.EscapeBicepString(propertyName), propertyValue);

    internal void AppendPropertyMatcher(string matchNotation, string value)
        => DoAppendProperty(matchNotation, value);

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
}
