// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Text.Json;

namespace Bicep.Core.Configuration;

public class ExperimentalFeaturesEnabledConfiguration : ConfigurationSection<ImmutableDictionary<string, bool>>
{
    public ExperimentalFeaturesEnabledConfiguration(JsonElement data) : base(data.EnumerateObject()
        .ToImmutableDictionary(p => p.Name, p => p.Value.GetBoolean(), StringComparer.OrdinalIgnoreCase)) {}

    public bool? IsEnabled(string featureName) => Data.TryGetValue(featureName, out var value) ? value : default;
}
