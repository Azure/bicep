// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json;
using Bicep.Core.Extensions;

namespace Bicep.Core.Configuration;

public partial class ImplicitExtensionsConfiguration : ConfigurationSection<ImmutableArray<string>>
{
    private ImplicitExtensionsConfiguration(ImmutableArray<string> data) : base(data) { }

    public static ImplicitExtensionsConfiguration Bind(JsonElement element)
        => new(element.TryGetStringArray() ?? []);

    public IEnumerable<string> GetImplicitExtensionNames() => this.Data;
}
