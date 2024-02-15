// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.Core.Extensions;

namespace Bicep.Core.Configuration;

public partial class ImplicitProvidersConfiguration : ConfigurationSection<string[]>
{
    private ImplicitProvidersConfiguration(string[] data) : base(data) { }

    public static ImplicitProvidersConfiguration Bind(JsonElement element)
        => new(element.TryGetStringArray()?.ToArray() ?? []);

    public IEnumerable<string> GetImplicitProviderNames() => this.Data;
}
