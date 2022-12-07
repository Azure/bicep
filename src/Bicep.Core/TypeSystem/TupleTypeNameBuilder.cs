// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Text;
using Bicep.Core.Parsing;

namespace Bicep.Core.TypeSystem;

internal class TupleTypeNameBuilder
{
    private const string EmptyTupleName = "<empty array>";
    private readonly StringBuilder builder = new("[");
    private bool hasItems = false;
    private bool finalized = false;

    internal void AppendItem(string item)
    {
        if (finalized)
        {
            throw new InvalidOperationException("AppendItem may not be called after the tuple type name has been finalized.");
        }

        if (hasItems)
        {
            builder.Append(", ");
        }
        hasItems = true;

        builder.Append(item);
    }

    public override string ToString()
    {
        if (!finalized)
        {
            builder.Append(']');
            finalized = true;
        }

        return hasItems ? builder.ToString() : EmptyTupleName;
    }
}
