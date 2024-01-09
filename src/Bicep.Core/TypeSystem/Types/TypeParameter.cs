// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.TypeSystem.Types;

public record TypeParameter(string Name, string Description, TypeSymbol? Type = null, bool Required = true)
{
    private readonly Lazy<string> lazySignature = new(
        () =>
        {
            StringBuilder builder = new();
            if (!Required)
            {
                builder.Append('[');
            }

            builder.Append(Name);

            if (Type is not null)
            {
                builder.Append(": ").Append(Type);
            }

            if (!Required)
            {
                builder.Append(']');
            }

            return builder.ToString();
        },
        LazyThreadSafetyMode.PublicationOnly);

    public override string ToString() => Name;

    public string Signature => lazySignature.Value;
}
