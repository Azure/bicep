// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Buffers;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Bicep.Core.Extensions;
using Bicep.Core.Json;

namespace Bicep.Core.Registry
{
    public class TemplateSpecEntity
    {
        public TemplateSpecEntity(ResourceIdentifier id, string name, ResourceType type, JsonElement systemData, string location, JsonElement? tags, string? description, JsonElement? linkedTemplates, JsonElement? metadata, JsonElement mainTemplate, JsonElement? uiFormDefinition)
        {
            this.Id = id;
            this.Name = name;
            this.Type = type;
            this.SystemData = systemData;
            this.Location = location;
            this.Tags = tags;
            this.Description = description;
            this.LinkedTemplates = linkedTemplates;
            this.Metadata = metadata;
            this.MainTemplate = mainTemplate;
            this.UiFormDefinition = uiFormDefinition;
        }

        public ResourceIdentifier Id { get; }

        public string Name { get; }

        public ResourceType Type { get; }

        public JsonElement SystemData { get; }

        public string Location { get; }

        public JsonElement? Tags { get; }

        public string? Description { get; }

        public JsonElement? LinkedTemplates { get; }

        public JsonElement? Metadata { get; }

        public JsonElement MainTemplate { get; }

        public JsonElement? UiFormDefinition { get; }

        public static TemplateSpecEntity FromSdkModel(TemplateSpecVersionData model) => new(
            model.Id,
            model.Name,
            model.Type,
            JsonElementFactory.CreateElement(model.SystemData),
            model.Location,
            JsonElementFactory.CreateNullableElement(model.Tags),
            model.Description,
            JsonElementFactory.CreateNullableElement(model.LinkedTemplates),
            JsonElementFactory.CreateNullableElement(model.Metadata),
            JsonElementFactory.CreateElement(model.MainTemplate),
            JsonElementFactory.CreateNullableElement(model.UiFormDefinition));

        public static TemplateSpecEntity FromJsonElement(JsonElement element)
        {
            string id = "";
            string name = "";
            string type = "";
            string location = "";
            JsonElement? tags = default;
            JsonElement systemData = default;

            // properties.*
            string? description = default;
            JsonElement? linkedTemplates = default;
            JsonElement? metadata = default;
            JsonElement mainTemplate = default;
            JsonElement? uiFormDefinition = default;

            foreach (var topLevelProperty in element.EnumerateObject())
            {
                switch (topLevelProperty.Name)
                {
                    case "id":
                        id = topLevelProperty.Value.ToNonNullString();
                        break;

                    case "name":
                        name = topLevelProperty.Value.ToNonNullString();
                        break;

                    case "type":
                        type = topLevelProperty.Value.ToNonNullString();
                        break;

                    case "location":
                        location = topLevelProperty.Value.ToNonNullString();
                        break;

                    case "tags" when topLevelProperty.Value.IsNotNullValue():
                        tags = topLevelProperty.Value;
                        break;

                    case "systemData":
                        systemData = topLevelProperty.Value;
                        break;

                    case "properties":
                        foreach (var property in topLevelProperty.Value.EnumerateObject())
                        {
                            switch (property.Name)
                            {
                                case "description":
                                    description = property.Value.GetString();
                                    break;

                                case "linkedTemplates" when property.Value.IsNotNullValue():
                                    linkedTemplates = property.Value;
                                    break;

                                case "metadata" when property.Value.IsNotNullValue():
                                    metadata = property.Value;
                                    break;

                                case "mainTemplate":
                                    mainTemplate = property.Value;
                                    break;

                                case "uiFormDefinition" when property.Value.IsNotNullValue():
                                    uiFormDefinition = property.Value;
                                    break;

                                default:
                                    break;
                            }
                        }

                        break;

                    default:
                        break;
                }
            }
            return new(id, name, type, systemData, location, tags, description, linkedTemplates, metadata, mainTemplate, uiFormDefinition);
        }

        public string ToUtf8Json()
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            var writterOptions = new JsonWriterOptions
            {
                // Using UnsafeRelaxedJsonEscaping to avoid escaping HTML-sensitive characters such as <, >, &, and '.
                // It is safe to use it because we are dealing with responses from the Azure SDK,
                // and the output will not be emitted into an HTML page or a <script> element.
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Indented = true,
            };

            using (var writer = new Utf8JsonWriter(bufferWriter, writterOptions))
            {
                // Top level properties.
                writer.WriteStartObject();

                writer.WritePropertyName("id");
                writer.WriteStringValue(this.Id);

                writer.WritePropertyName("name");
                writer.WriteStringValue(this.Name);

                writer.WritePropertyName("type");
                writer.WriteStringValue(this.Type);

                writer.WritePropertyName("location");
                writer.WriteStringValue(this.Location);

                if (this.Tags is { } tags)
                {
                    writer.WritePropertyName("tags");
                    tags.WriteTo(writer);
                }

                writer.WritePropertyName("systemData");
                this.SystemData.WriteTo(writer);

                // properites.*
                writer.WritePropertyName("properties");
                {
                    writer.WriteStartObject();

                    if (this.Description is { } description)
                    {
                        writer.WritePropertyName("description");
                        writer.WriteStringValue(Description);
                    }

                    if (this.LinkedTemplates is { } linkedTemplates)
                    {
                        writer.WritePropertyName("linkedTemplates");
                        linkedTemplates.WriteTo(writer);
                    }

                    if (this.Metadata is { } metadata)
                    {
                        writer.WritePropertyName("metadata");
                        metadata.WriteTo(writer);
                    }

                    writer.WritePropertyName("mainTemplate");
                    this.MainTemplate.WriteTo(writer);

                    if (this.UiFormDefinition is { } uiFormDefinition)
                    {
                        writer.WritePropertyName("uiFormDefinition");
                        uiFormDefinition.WriteTo(writer);
                    }
                    writer.WriteEndObject();
                }

                writer.WriteEndObject();
            }

            return Encoding.UTF8.GetString(bufferWriter.WrittenSpan);
        }
    }
}
