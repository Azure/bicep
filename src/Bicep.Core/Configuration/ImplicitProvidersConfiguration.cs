// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json;
using Bicep.Core.Extensions;

namespace Bicep.Core.Configuration;

public partial class ImplicitProvidersConfiguration : ConfigurationSection<ImmutableArray<string>>
{
    private ImplicitProvidersConfiguration(ImmutableArray<string> data) : base(data) { }

    public static ImplicitProvidersConfiguration Bind(JsonElement element)
        => new(element.TryGetStringArray() ?? []);

    public IEnumerable<string> GetImplicitProviderNames() => this.Data;
}
