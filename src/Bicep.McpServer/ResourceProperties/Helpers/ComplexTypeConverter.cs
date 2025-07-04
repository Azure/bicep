// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.McpServer.ResourceProperties.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bicep.McpServer.ResourceProperties.Helpers;

public class ComplexTypeConverter : JsonConverter<ComplexType>
{
    private const string TypeProperty = "type";
    private const string DataProperty = "data";

    public override ComplexType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            if (!doc.RootElement.TryGetProperty(TypeProperty, out JsonElement typeElement))
            {
                throw new JsonException("Type property not found");
            }

            if (!doc.RootElement.TryGetProperty(DataProperty, out JsonElement dataElement))
            {
                throw new JsonException("Data property not found");
            }

            string type = typeElement.GetString() ?? "Unexpected type";
            switch (type)
            {
                case nameof(ResourceTypeEntity):
                    var resourceTypeEntity = JsonSerializer.Deserialize<ResourceTypeEntity>(dataElement.GetRawText(), options) ?? throw new JsonException($"Failed to deserialize ResourceTypeEntity. Raw text: {dataElement.GetRawText()}");
                    return resourceTypeEntity;
                case nameof(ResourceFunctionTypeEntity):
                    var resourceFunctionTypeEntity = JsonSerializer.Deserialize<ResourceFunctionTypeEntity>(dataElement.GetRawText(), options) ?? throw new JsonException($"Failed to deserialize ResourceFunctionTypeEntity. Raw text: {dataElement.GetRawText()}");
                    return resourceFunctionTypeEntity;
                case nameof(ObjectTypeEntity):
                    var objectTypeEntity = JsonSerializer.Deserialize<ObjectTypeEntity>(dataElement.GetRawText(), options) ?? throw new JsonException($"Failed to deserialize ObjectTypeEntity. Raw text: {dataElement.GetRawText()}");
                    return objectTypeEntity;
                case nameof(DiscriminatedObjectTypeEntity):
                    var discriminatedObjectTypeEntity = JsonSerializer.Deserialize<DiscriminatedObjectTypeEntity>(dataElement.GetRawText(), options) ?? throw new JsonException($"Failed to deserialize DiscriminatedObjectTypeEntity. Raw text: {dataElement.GetRawText()}");
                    return discriminatedObjectTypeEntity;
                default:
                    throw new NotSupportedException($"Type {type} is not supported");
            }
        }
    }

    public override void Write(Utf8JsonWriter writer, ComplexType value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString(TypeProperty, value.GetType().Name);

        writer.WritePropertyName(DataProperty);
        switch (value)
        {
            case ResourceTypeEntity resourceTypeEntity:
                JsonSerializer.Serialize(writer, resourceTypeEntity, options);
                break;
            case ResourceFunctionTypeEntity resourceFunctionTypeEntity:
                JsonSerializer.Serialize(writer, resourceFunctionTypeEntity, options);
                break;
            case ObjectTypeEntity objectTypeEntity:
                JsonSerializer.Serialize(writer, objectTypeEntity, options);
                break;
            case DiscriminatedObjectTypeEntity discriminatedObjectTypeEntity:
                JsonSerializer.Serialize(writer, discriminatedObjectTypeEntity, options);
                break;
            default:
                throw new NotSupportedException($"Type of ComplexType {value.Name} is not supported");
        }

        writer.WriteEndObject();
    }
}
