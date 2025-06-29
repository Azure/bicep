// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Json;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem.Providers.Extensibility;

namespace Bicep.Core.Configuration;

public record ExtensionConfigEntry
{
    public bool BuiltIn => this.Value == "builtin:";

    public string Value { get; }

    public ExtensionConfigEntry(string value)
    {
        Value = value;
    }

    public override string ToString()
        => Value;
}

public partial class ExtensionsConfiguration : ConfigurationSection<ImmutableDictionary<string, ExtensionConfigEntry>>
{
    private ExtensionsConfiguration(ImmutableDictionary<string, ExtensionConfigEntry> data) : base(data) { }

    public static ExtensionsConfiguration Bind(JsonElement element)
        => new(element.ToNonNullObject<ImmutableDictionary<string, string>>()
            .ToImmutableDictionary(
                pair => pair.Key,
                pair => new ExtensionConfigEntry(pair.Value))
        );

    public ResultWithDiagnosticBuilder<ExtensionConfigEntry> TryGetExtensionSource(string extensionName)
    {
        if (!this.Data.TryGetValue(extensionName, out var extensionConfigEntry))
        {
            if (LanguageConstants.IdentifierComparer.Equals(extensionName, MicrosoftGraphExtensionFacts.builtInExtensionName))
            {
                return new(x => x.MicrosoftGraphBuiltinRetired(null));
            }

            return new(x => x.UnrecognizedExtension(extensionName));
        }
        return new(extensionConfigEntry);
    }

    public override void WriteTo(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        foreach (var (key, value) in this.Data)
        {
            writer.WritePropertyName(key);
            writer.WriteStringValue(value.ToString());
        }
        writer.WriteEndObject();
    }

    public bool IsSysOrBuiltIn(string extensionName)
        => extensionName == SystemNamespaceType.BuiltInName || this.Data.TryGetValue(extensionName)?.BuiltIn == true;
}
