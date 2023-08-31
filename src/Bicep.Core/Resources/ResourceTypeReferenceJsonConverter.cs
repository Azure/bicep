// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bicep.Core.Resources;

public class ResourceTypeReferenceJsonConverter : JsonConverter<ResourceTypeReference>
{
    public override ResourceTypeReference? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.GetString() is { } stringValue ? ResourceTypeReference.Parse(stringValue) : null;

    public override void Write(Utf8JsonWriter writer, ResourceTypeReference value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString());

    public override ResourceTypeReference ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => ResourceTypeReference.Parse(reader.GetString()!);

    public override void WriteAsPropertyName(Utf8JsonWriter writer, ResourceTypeReference value, JsonSerializerOptions options)
        => writer.WritePropertyName(value.ToString());
}
