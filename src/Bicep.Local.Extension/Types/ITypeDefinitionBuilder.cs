// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Azure.Bicep.Types.Index;

namespace Bicep.Local.Extension.Types;

[JsonSerializable(typeof(TypeDefinition))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class TypeDefinitionSerializationContext : JsonSerializerContext { }

public record TypeDefinition(string IndexJson, ImmutableDictionary<string, string> TypesJsons);

public interface ITypeDefinitionBuilder
{
    TypeSettings Settings { get; }
    TypeDefinition GenerateBicepResourceTypes();
}
