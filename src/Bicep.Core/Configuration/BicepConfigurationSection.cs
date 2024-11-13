// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.Core.Extensions;

namespace Bicep.Core.Configuration;

public record BicepConfiguration(
    string? Version);

public class BicepConfigurationSection : ConfigurationSection<BicepConfiguration>
{
    public BicepConfigurationSection(BicepConfiguration data)
        : base(data)
    {
    }

    public static BicepConfigurationSection Bind(JsonElement element)
        => new(element.ToNonNullObject<BicepConfiguration>());
}
